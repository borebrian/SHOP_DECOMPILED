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

import Crc32 from "./crc32.js";
import { Encrypt, Decrypt, ERR_INVALID_PASSWORD } from "./crypto.js";

const CODEC_DEFLATE = "deflate";
const CODEC_INFLATE = "inflate";
const ERR_INVALID_SIGNATURE = "Invalid signature";

class Inflate {

	constructor(options) {
		this.signature = options.inputSignature;
		this.encrypted = Boolean(options.inputPassword);
		this.signed = options.inputSigned;
		this.compressed = options.inputCompressed;
		this.inflate = options.inputCompressed && new options.codecConstructor();
		this.crc32 = options.inputSigned && new Crc32();
		this.decrypt = this.encrypted && new Decrypt(options.inputPassword, options.inputSigned, options.inputEncryptionStrength);
	}

	async append(data) {
		if (this.encrypted) {
			data = await this.decrypt.append(data);
		}
		if (this.compressed && data.length) {
			data = await this.inflate.append(data);
		}
		if (!this.encrypted && this.signed) {
			this.crc32.append(data);
		}
		return data;
	}

	async flush() {
		let signature, data = new Uint8Array(0);
		if (this.encrypted) {
			const result = await this.decrypt.flush();
			if (!result.valid) {
				throw new Error(ERR_INVALID_SIGNATURE);
			}
			data = result.data;
		} else if (this.signed) {
			const dataViewSignature = new DataView(new Uint8Array(4).buffer);
			signature = this.crc32.get();
			dataViewSignature.setUint32(0, signature);
			if (this.signature != dataViewSignature.getUint32(0, false)) {
				throw new Error(ERR_INVALID_SIGNATURE);
			}
		}
		if (this.compressed) {
			data = (await this.inflate.append(data)) || new Uint8Array(0);
			await this.inflate.flush();
		}
		return { data, signature };
	}
}

class Deflate {

	constructor(options) {
		this.encrypted = options.outputEncrypted;
		this.signed = options.outputSigned;
		this.compressed = options.outputCompressed;
		this.deflate = options.outputCompressed && new options.codecConstructor({ level: options.level || 5 });
		this.crc32 = options.outputSigned && new Crc32();
		this.encrypt = this.encrypted && new Encrypt(options.outputPassword, options.outputEncryptionStrength);
	}

	async append(inputData) {
		let data = inputData;
		if (this.compressed && inputData.length) {
			data = await this.deflate.append(inputData);
		}
		if (this.encrypted) {
			data = await this.encrypt.append(data);
		} else if (this.signed) {
			this.crc32.append(inputData);
		}
		return data;
	}

	async flush() {
		let data = new Uint8Array(0), signature;
		if (this.compressed) {
			data = (await this.deflate.flush()) || new Uint8Array(0);
		}
		if (this.encrypted) {
			data = await this.encrypt.append(data);
			const result = await this.encrypt.flush();
			signature = result.signature;
			const newData = new Uint8Array(data.length + result.data.length);
			newData.set(data, 0);
			newData.set(result.data, data.length);
			data = newData;
		} else if (this.signed) {
			signature = this.crc32.get();
		}
		return { data, signature };
	}
}

export { Inflate, Deflate, createCodec, CODEC_DEFLATE, CODEC_INFLATE, ERR_INVALID_SIGNATURE, ERR_INVALID_PASSWORD };

function createCodec(options) {
	if (options.codecType.startsWith(CODEC_DEFLATE)) {
		return new Deflate(options);
	} else if (options.codecType.startsWith(CODEC_INFLATE)) {
		return new Inflate(options);
	}
}