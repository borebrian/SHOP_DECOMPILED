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

/* global navigator */

"use strict";

import { default as initShimAsyncCodec } from "./util/stream-codec-shim.js";
import { ZipReader as BaseZipReader } from "./zip-reader.js";
import { ZipWriter as BaseZipWriter } from "./zip-writer.js";

const DEFAULT_CONFIGURATION = {
	chunkSize: 512 * 1024,
	maxWorkers: (typeof navigator != "undefined" && navigator.hardwareConcurrency) || 2,
	useWebWorkers: true,
	workerScripts: undefined
};

let config = Object.assign({}, DEFAULT_CONFIGURATION);

class ZipReader extends BaseZipReader {

	constructor(reader, options) {
		super(reader, options, config);
	}
}

class ZipWriter extends BaseZipWriter {

	constructor(writer, options) {
		super(writer, options, config);
	}
}

export * from "./stream.js";
export {
	ERR_BAD_FORMAT,
	ERR_EOCDR_NOT_FOUND,
	ERR_EOCDR_ZIP64_NOT_FOUND,
	ERR_EOCDR_LOCATOR_ZIP64_NOT_FOUND,
	ERR_CENTRAL_DIRECTORY_NOT_FOUND,
	ERR_LOCAL_FILE_HEADER_NOT_FOUND,
	ERR_EXTRAFIELD_ZIP64_NOT_FOUND,
	ERR_ENCRYPTED,
	ERR_UNSUPPORTED_COMPRESSION,
	ERR_INVALID_SIGNATURE,
	ERR_INVALID_PASSWORD
} from "./zip-reader.js";
export {
	ERR_DUPLICATED_NAME,
	ERR_INVALID_COMMENT,
	ERR_INVALID_ENTRY_NAME,
	ERR_INVALID_ENTRY_COMMENT,
	ERR_INVALID_VERSION,
	ERR_INVALID_DATE,
	ERR_INVALID_EXTRAFIELD_TYPE,
	ERR_INVALID_EXTRAFIELD_DATA,
	ERR_INVALID_ENCRYPTION_STRENGTH
} from "./zip-writer.js";
export {
	configure,
	initShimAsyncCodec,
	getMimeType,
	ZipReader,
	ZipWriter
};

function configure(configuration) {
	if (configuration.chunkSize !== undefined) {
		config.chunkSize = configuration.chunkSize;
	}
	if (configuration.maxWorkers !== undefined) {
		config.maxWorkers = configuration.maxWorkers;
	}
	if (configuration.useWebWorkers !== undefined) {
		config.useWebWorkers = configuration.useWebWorkers;
	}
	if (configuration.Deflate !== undefined) {
		config.Deflate = configuration.Deflate;
	}
	if (configuration.Inflate !== undefined) {
		config.Inflate = configuration.Inflate;
	}
	if (configuration.workerScripts !== undefined) {
		if (configuration.workerScripts.deflate) {
			if (!Array.isArray(configuration.workerScripts.deflate)) {
				throw new Error("workerScripts.deflate must be an array");
			}
			if (!config.workerScripts) {
				config.workerScripts = {};
			}
			config.workerScripts.deflate = configuration.workerScripts.deflate;
		}
		if (configuration.workerScripts.inflate) {
			if (!Array.isArray(configuration.workerScripts.inflate)) {
				throw new Error("workerScripts.inflate must be an array");
			}
			if (!config.workerScripts) {
				config.workerScripts = {};
			}
			config.workerScripts.inflate = configuration.workerScripts.inflate;
		}
	}
}

function getMimeType() {
	return "application/octet-stream";
}