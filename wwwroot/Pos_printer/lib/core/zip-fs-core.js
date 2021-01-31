/*
 Copyright (c) 2021 Gildas Lormeau. All rights reserved.

 Redistribution and use in source and binary forms, with or without
 modification, are permitted provided that the following conditions are met:

 1. Redistributions of source code must retain the above copyright notice,
 this list of conditions and the following disclaimer.

 2. Redistributions in binary form must reproduce the above copyright 
 notice, this list of conditions and the following disclaimer in 
 the documentation and/or other materials provided with the distribution.

 3. The names of the authors may not be used to endorse or promote products
 derived from this software without specific prior written permission.

 THIS SOFTWARE IS PROVIDED ''AS IS'' AND ANY EXPRESSED OR IMPLIED WARRANTIES,
 INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
 FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL JCRAFT,
 INC. OR ANY CONTRIBUTORS TO THIS SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT,
 INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
 OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
 EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

"use strict";

import {
	Reader,
	ZipReader,
	ZipWriter,
	TextReader,
	TextWriter,
	Data64URIReader,
	Data64URIWriter,
	BlobReader,
	BlobWriter,
	HttpReader,
	HttpRangeReader
} from "./zip-core.js";
export * from "./zip-core.js";

const CHUNK_SIZE = 512 * 1024;

class ZipEntry {
	constructor(fs, name, params, parent) {
		if (fs.root && parent && parent.getChildByName(name)) {
			throw new Error("Entry filename already exists");
		}
		if (!params) {
			params = {};
		}
		this.fs = fs;
		this.name = name;
		this.data = params.data;
		this.id = fs.entries.length;
		this.parent = parent;
		this.children = [];
		this.uncompressedSize = 0;
		fs.entries.push(this);
		if (parent) {
			this.parent.children.push(this);
		}
	}
	moveTo(target) {
		// deprecated
		this.fs.move(this, target);
	}
	getFullname() {
		return this.getRelativeName();
	}
	getRelativeName(ancestor = this.fs.root) {
		let relativeName = this.name, entry = this.parent;
		while (entry && entry != ancestor) {
			relativeName = (entry.name ? entry.name + "/" : "") + relativeName;
			entry = entry.parent;
		}
		return relativeName;
	}
	isDescendantOf(ancestor) {
		let entry = this.parent;
		while (entry && entry.id != ancestor.id) {
			entry = entry.parent;
		}
		return Boolean(entry);
	}
}

class ZipFileEntry extends ZipEntry {
	constructor(fs, name, params, parent) {
		super(fs, name, params, parent);
		this.Reader = params.Reader;
		this.Writer = params.Writer;
		if (params.getData) {
			this.getData = params.getData;
		}
	}
	async getData(writer, options = {}) {
		if (!writer || (writer.constructor == this.Writer && this.data)) {
			return this.data;
		} else {
			this.reader = new this.Reader(this.data, options);
			await this.reader.init();
			await writer.init();
			this.uncompressedSize = this.reader.size;
			return bufferedCopy(this.reader, writer);
		}
	}
	getText(encoding, options) {
		return this.getData(new TextWriter(encoding), options);
	}
	getBlob(mimeType, options) {
		return this.getData(new BlobWriter(mimeType), options);
	}
	getData64URI(mimeType, options) {
		return this.getData(new Data64URIWriter(mimeType), options);
	}
	replaceBlob(blob) {
		this.data = blob;
		this.Reader = BlobReader;
		this.reader = null;
	}
	replaceText(text) {
		this.data = text;
		this.Reader = TextReader;
		this.reader = null;
	}
	replaceData64URI(dataURI) {
		this.data = dataURI;
		this.Reader = Data64URIReader;
		this.reader = null;
	}
}

class ZipDirectoryEntry extends ZipEntry {
	constructor(fs, name, params, parent) {
		super(fs, name, params, parent);
		this.directory = true;
	}
	addDirectory(name) {
		return addChild(this, name, null, true);
	}
	addText(name, text) {
		return addChild(this, name, {
			data: text,
			Reader: TextReader,
			Writer: TextWriter
		});
	}
	addBlob(name, blob) {
		return addChild(this, name, {
			data: blob,
			Reader: BlobReader,
			Writer: BlobWriter
		});
	}
	addData64URI(name, dataURI) {
		return addChild(this, name, {
			data: dataURI,
			Reader: Data64URIReader,
			Writer: Data64URIWriter
		});
	}
	addHttpContent(name, url, options = {}) {
		return addChild(this, name, {
			data: url,
			Reader: options.useRangeHeader ? HttpRangeReader : HttpReader
		});
	}
	async addFileSystemEntry(fileSystemEntry) {
		return addFileSystemEntry(this, fileSystemEntry);
	}
	async addData(name, params) {
		return addChild(this, name, params);
	}
	async importBlob(blob, options = {}) {
		await this.importZip(new BlobReader(blob), options);
	}
	async importData64URI(dataURI, options = {}) {
		await this.importZip(new Data64URIReader(dataURI), options);
	}
	async importHttpContent(URL, options = {}) {
		await this.importZip(options.useRangeHeader ? new HttpRangeReader(URL) : new HttpReader(URL), options);
	}
	async exportBlob(options = {}) {
		return this.exportZip(new BlobWriter("application/zip"), options);
	}
	async exportData64URI(options = {}) {
		return this.exportZip(new Data64URIWriter("application/zip"), options);
	}
	async importZip(reader, options) {
		await reader.init();
		const zipReader = new ZipReader(reader, options);
		const entries = await zipReader.getEntries();
		entries.forEach((entry) => {
			let parent = this, path = entry.filename.split("/"), name = path.pop();
			path.forEach(pathPart => parent = parent.getChildByName(pathPart) || new ZipDirectoryEntry(this.fs, pathPart, null, parent));
			if (!entry.directory) {
				addChild(parent, name, {
					data: entry,
					Reader: getZipBlobReader(Object.assign({}, options))
				});
			}
		});
	}
	async exportZip(writer, options) {
		await initReaders(this);
		const zipWriter = new ZipWriter(writer, options);
		await exportZip(zipWriter, this, getTotalSize([this], "uncompressedSize"), options);
		await zipWriter.close();
		return writer.getData();
	}
	getChildByName(name) {
		for (let childIndex = 0; childIndex < this.children.length; childIndex++) {
			const child = this.children[childIndex];
			if (child.name == name)
				return child;
		}
	}
}


class FS {
	constructor() {
		resetFS(this);
	}
	get children() {
		return this.root.children;
	}
	remove(entry) {
		detach(entry);
		this.entries[entry.id] = null;
	}
	move(entry, destination) {
		if (entry == this.root) {
			throw new Error("Root directory cannot be moved");
		} else {
			if (destination.directory) {
				if (!destination.isDescendantOf(entry)) {
					if (entry != destination) {
						if (destination.getChildByName(entry.name)) {
							throw new Error("Entry filename already exists");
						}
						detach(entry);
						entry.parent = destination;
						destination.children.push(entry);
					}
				} else {
					throw new Error("Entry is a ancestor of target entry");
				}
			} else {
				throw new Error("Target entry is not a directory");
			}
		}
	}
	find(fullname) {
		const path = fullname.split("/");
		let node = this.root;
		for (let index = 0; node && index < path.length; index++) {
			node = node.getChildByName(path[index]);
		}
		return node;
	}
	getById(id) {
		return this.entries[id];
	}
	getChildByName(name) {
		return this.root.getChildByName(name);
	}
	addDirectory(name) {
		return this.root.addDirectory(name);
	}
	addText(name, text) {
		return this.root.addText(name, text);
	}
	addBlob(name, blob) {
		return this.root.addBlob(name, blob);
	}
	addData64URI(name, dataURI) {
		return this.root.addData64URI(name, dataURI);
	}
	addHttpContent(name, url, options) {
		return this.root.addHttpContent(name, url, options);
	}
	async addFileSystemEntry(fileSystemEntry) {
		return this.root.addFileSystemEntry(fileSystemEntry);
	}
	async addData(name, params) {
		return this.root.addData(name, params);
	}
	async importBlob(blob, options) {
		resetFS(this);
		await this.root.importBlob(blob, options);
	}
	async importData64URI(dataURI, options) {
		resetFS(this);
		await this.root.importData64URI(dataURI, options);
	}
	async importHttpContent(url, options) {
		resetFS(this);
		await this.root.importHttpContent(url, options);
	}
	async exportBlob(options) {
		return this.root.exportBlob(options);
	}
	async exportData64URI(options) {
		return this.root.exportData64URI(options);
	}
}

const fs = { FS, ZipDirectoryEntry, ZipFileEntry };
export { fs };

function getTotalSize(entries, propertyName) {
	let size = 0;
	entries.forEach(process);
	return size;

	function process(entry) {
		size += entry[propertyName];
		if (entry.children) {
			entry.children.forEach(process);
		}
	}
}

function getZipBlobReader(options) {
	return class extends Reader {

		constructor(entry, options = {}) {
			super();
			this.entry = entry;
			this.options = options;
		}

		async readUint8Array(index, length) {
			if (!this.blobReader) {
				const data = await this.entry.getData(new BlobWriter(), Object.assign({}, this.options, options));
				this.data = data;
				this.blobReader = new BlobReader(data);
			}
			return this.blobReader.readUint8Array(index, length);
		}

		async init() {
			this.size = this.entry.uncompressedSize;
		}
	};
}

async function initReaders(entry) {
	if (entry.children.length) {
		for (const child of entry.children) {
			if (child.directory) {
				await initReaders(child);
			} else {
				child.reader = new child.Reader(child.data);
				await child.reader.init();
				child.uncompressedSize = child.reader.size;
			}
		}
	}
}

function detach(entry) {
	const children = entry.parent.children;
	children.forEach((child, index) => {
		if (child.id == entry.id)
			children.splice(index, 1);
	});
}

async function exportZip(zipWriter, entry, totalSize, options) {
	const selectedEntry = entry;
	const entryOffsets = new Map();
	await process(zipWriter, entry);

	async function process(zipWriter, entry) {
		await exportChild();

		async function exportChild() {
			if (options.bufferedWrite) {
				await Promise.all(entry.children.map(processChild));
			} else {
				for (const child of entry.children) {
					await processChild(child);
				}
			}
		}

		async function processChild(child) {
			const name = options.relativePath ? child.getRelativeName(selectedEntry) : child.getFullname();
			await zipWriter.add(name, child.reader, Object.assign({
				directory: child.directory
			}, Object.assign({}, options, {
				onprogress: indexProgress => {
					if (options.onprogress) {
						entryOffsets.set(name, indexProgress);
						options.onprogress(Array.from(entryOffsets.values()).reduce((previousValue, currentValue) => previousValue + currentValue), totalSize);
					}
				}
			})));
			await process(zipWriter, child);
		}
	}
}

async function addFileSystemEntry(zipEntry, fileSystemEntry) {
	if (fileSystemEntry.isDirectory) {
		const entry = zipEntry.addDirectory(fileSystemEntry.name);
		await addDirectory(entry, fileSystemEntry);
		return entry;
	} else {
		return new Promise((resolve, reject) => fileSystemEntry.file(file => resolve(zipEntry.addBlob(fileSystemEntry.name, file)), reject));
	}

	async function addDirectory(zipEntry, fileEntry) {
		const children = await getChildren(fileEntry);
		for (const child of children) {
			if (child.isDirectory) {
				await addDirectory(zipEntry.addDirectory(child.name), child);
			} else {
				await new Promise((resolve, reject) => {
					child.file(file => {
						const childZipEntry = zipEntry.addBlob(child.name, file);
						childZipEntry.uncompressedSize = file.size;
						resolve(childZipEntry);
					}, reject);
				});
			}
		}
	}

	function getChildren(fileEntry) {
		return new Promise((resolve, reject) => {
			let entries = [];
			if (fileEntry.isDirectory) {
				readEntries(fileEntry.createReader());
			}
			if (fileEntry.isFile) {
				resolve(entries);
			}

			function readEntries(directoryReader) {
				directoryReader.readEntries(temporaryEntries => {
					if (!temporaryEntries.length) {
						resolve(entries);
					} else {
						entries = entries.concat(temporaryEntries);
						readEntries(directoryReader);
					}
				}, reject);
			}
		});
	}
}

function resetFS(fs) {
	fs.entries = [];
	fs.root = new ZipDirectoryEntry(fs);
}

async function bufferedCopy(reader, writer) {
	return stepCopy();

	async function stepCopy(chunkIndex = 0) {
		const index = chunkIndex * CHUNK_SIZE;
		if (index < reader.size) {
			const array = await reader.readUint8Array(index, Math.min(CHUNK_SIZE, reader.size - index));
			await writer.writeUint8Array(array);
			return stepCopy(chunkIndex + 1);
		} else {
			return writer.getData();
		}
	}
}

function addChild(parent, name, params, directory) {
	if (parent.directory) {
		return directory ? new ZipDirectoryEntry(parent.fs, name, params, parent) : new ZipFileEntry(parent.fs, name, params, parent);
	} else {
		throw new Error("Parent entry is not a directory");
	}
}