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

/* global crypto, TextEncoder */

"use strict";

const ERR_INVALID_PASSWORD = "Invalid pasword";
const BLOCK_LENGTH = 16;
const RAW_FORMAT = "raw";
const PBKDF2_ALGORITHM = { name: "PBKDF2" };
const SIGNATURE_ALGORITHM = { name: "HMAC" };
const HASH_FUNCTION = "SHA-1";
const CRYPTO_KEY_ALGORITHM = { name: "AES-CTR" };
const BASE_KEY_ALGORITHM = Object.assign({ hash: SIGNATURE_ALGORITHM }, PBKDF2_ALGORITHM);
const DERIVED_BITS_ALGORITHM = Object.assign({ iterations: 1000, hash: { name: HASH_FUNCTION } }, PBKDF2_ALGORITHM);
const AUTHENTICATION_ALGORITHM = Object.assign({ hash: HASH_FUNCTION }, SIGNATURE_ALGORITHM);
const CRYPTO_ALGORITHM = Object.assign({ length: BLOCK_LENGTH }, CRYPTO_KEY_ALGORITHM);
const DERIVED_BITS_USAGE = ["deriveBits"];
const SIGN_USAGE = ["sign"];
const SALT_LENGTH = [8, 12, 16];
const KEY_LENGTH = [16, 24, 32];
const SIGNATURE_LENGTH = 10;
const COUNTER_DEFAULT_VALUE = [1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
const subtle = crypto.subtle;

class Decrypt {

	constructor(password, signed, strength) {
		this.password = password;
		this.signed = signed;
		this.strength = strength - 1;
		this.input = signed && new Uint8Array(0);
		this.pendingInput = new Uint8Array(0);
	}

	async append(input) {
		const decrypt = async (offset = 0) => {
			if (offset + BLOCK_LENGTH <= bufferedInput.length - SIGNATURE_LENGTH) {
				const chunkToDecrypt = bufferedInput.subarray(offset, offset + BLOCK_LENGTH);
				const outputChunk = await subtle.decrypt(Object.assign({ counter: this.counter }, CRYPTO_ALGORITHM), this.keys.key, chunkToDecrypt);
				incrementCounter(this.counter);
				output.set(new Uint8Array(outputChunk), offset);
				return decrypt(offset + BLOCK_LENGTH);
			} else {
				this.pendingInput = bufferedInput.subarray(offset);
				if (this.signed) {
					this.input = concat(this.input, input);
				}
				return output;
			}
		};

		if (this.password) {
			const preambule = input.subarray(0, SALT_LENGTH[this.strength] + 2);
			await createDecryptionKeys(this, preambule, this.password);
			this.password = null;
			input = input.subarray(SALT_LENGTH[this.strength] + 2);
		}
		let output = new Uint8Array(input.length - SIGNATURE_LENGTH - ((input.length - SIGNATURE_LENGTH) % BLOCK_LENGTH));
		let bufferedInput = input;
		if (this.pendingInput.length) {
			bufferedInput = concat(this.pendingInput, input);
			output = expand(output, bufferedInput.length - SIGNATURE_LENGTH - ((bufferedInput.length - SIGNATURE_LENGTH) % BLOCK_LENGTH));
		}
		return decrypt();
	}

	async flush() {
		const pendingInput = this.pendingInput;
		const keys = this.keys;
		const chunkToDecrypt = pendingInput.subarray(0, pendingInput.length - SIGNATURE_LENGTH);
		const originalSignatureArray = pendingInput.subarray(pendingInput.length - SIGNATURE_LENGTH);
		let decryptedChunkArray = new Uint8Array(0);
		if (chunkToDecrypt.length) {
			const decryptedChunk = await subtle.decrypt(Object.assign({ counter: this.counter }, CRYPTO_ALGORITHM), keys.key, chunkToDecrypt);
			decryptedChunkArray = new Uint8Array(decryptedChunk);
		}
		let valid = true;
		if (this.signed) {
			const signature = await subtle.sign(SIGNATURE_ALGORITHM, keys.authentication, this.input.subarray(0, this.input.length - SIGNATURE_LENGTH));
			const signatureArray = new Uint8Array(signature);
			this.input = null;
			for (let indexSignature = 0; indexSignature < SIGNATURE_LENGTH; indexSignature++) {
				if (signatureArray[indexSignature] != originalSignatureArray[indexSignature]) {
					valid = false;
				}
			}
		}
		return {
			valid,
			data: decryptedChunkArray
		};
	}

}

class Encrypt {

	constructor(password, strength) {
		this.password = password;
		this.strength = strength - 1;
		this.output = new Uint8Array(0);
		this.pendingInput = new Uint8Array(0);
	}

	async append(input) {
		const encrypt = async (offset = 0) => {
			if (offset + BLOCK_LENGTH <= input.length) {
				const chunkToEncrypt = input.subarray(offset, offset + BLOCK_LENGTH);
				const outputChunk = await subtle.encrypt(Object.assign({ counter: this.counter }, CRYPTO_ALGORITHM), this.keys.key, chunkToEncrypt);
				incrementCounter(this.counter);
				output.set(new Uint8Array(outputChunk), offset + preambule.length);
				return encrypt(offset + BLOCK_LENGTH);
			} else {
				this.pendingInput = input.subarray(offset);
				this.output = concat(this.output, output);
				return output;
			}
		};

		let preambule = new Uint8Array(0);
		if (this.password) {
			preambule = await createEncryptionKeys(this, this.password);
			this.password = null;
		}
		let output = new Uint8Array(preambule.length + input.length - (input.length % BLOCK_LENGTH));
		output.set(preambule, 0);
		if (this.pendingInput.length) {
			input = concat(this.pendingInput, input);
			output = expand(output, input.length - (input.length % BLOCK_LENGTH));
		}
		return encrypt();
	}

	async flush() {
		let encryptedChunkArray = new Uint8Array(0);
		if (this.pendingInput.length) {
			const encryptedChunk = await subtle.encrypt(Object.assign({ counter: this.counter }, CRYPTO_ALGORITHM), this.keys.key, this.pendingInput);
			encryptedChunkArray = new Uint8Array(encryptedChunk);
			this.output = concat(this.output, encryptedChunkArray);
		}
		const signature = await subtle.sign(SIGNATURE_ALGORITHM, this.keys.authentication, this.output.subarray(SALT_LENGTH[this.strength] + 2));
		this.output = null;
		const signatureArray = new Uint8Array(signature).subarray(0, SIGNATURE_LENGTH);
		return {
			data: concat(encryptedChunkArray, signatureArray),
			signature: signatureArray
		};
	}
}

export {
	Decrypt,
	Encrypt,
	ERR_INVALID_PASSWORD
};

async function createDecryptionKeys(decrypt, preambuleArray, password) {
	await createKeys(decrypt, password, preambuleArray.subarray(0, SALT_LENGTH[decrypt.strength]), ["decrypt"]);
	const passwordVerification = preambuleArray.subarray(SALT_LENGTH[decrypt.strength]);
	const passwordVerificationKey = decrypt.keys.passwordVerification;
	if (passwordVerificationKey[0] != passwordVerification[0] || passwordVerificationKey[1] != passwordVerification[1]) {
		throw new Error(ERR_INVALID_PASSWORD);
	}
}

async function createEncryptionKeys(encrypt, password) {
	const salt = crypto.getRandomValues(new Uint8Array(SALT_LENGTH[encrypt.strength]));
	await createKeys(encrypt, password, salt, ["encrypt"]);
	return concat(salt, encrypt.keys.passwordVerification);
}

async function createKeys(target, password, salt, keyUsage) {
	target.counter = new Uint8Array(COUNTER_DEFAULT_VALUE);
	const encodedPassword = (new TextEncoder()).encode(password);
	const basekey = await subtle.importKey(RAW_FORMAT, encodedPassword, BASE_KEY_ALGORITHM, false, DERIVED_BITS_USAGE);
	const derivedBits = await subtle.deriveBits(Object.assign({ salt }, DERIVED_BITS_ALGORITHM), basekey, 8 * ((KEY_LENGTH[target.strength] * 2) + 2));
	const compositeKey = new Uint8Array(derivedBits);
	target.keys = {
		key: await subtle.importKey(RAW_FORMAT, compositeKey.subarray(0, KEY_LENGTH[target.strength]), CRYPTO_KEY_ALGORITHM, true, keyUsage),
		authentication: await subtle.importKey(RAW_FORMAT, compositeKey.subarray(KEY_LENGTH[target.strength], KEY_LENGTH[target.strength] * 2), AUTHENTICATION_ALGORITHM, false, SIGN_USAGE),
		passwordVerification: compositeKey.subarray(KEY_LENGTH[target.strength] * 2)
	};
}

function incrementCounter(counter) {
	for (let indexCounter = 0; indexCounter < 16; indexCounter++) {
		if (counter[indexCounter] == 255) {
			counter[indexCounter] = 0;
		} else {
			counter[indexCounter]++;
			break;
		}
	}
}

function concat(leftArray, rightArray) {
	let array = leftArray;
	if (leftArray.length + rightArray.length) {
		array = new Uint8Array(leftArray.length + rightArray.length);
		array.set(leftArray, 0);
		array.set(rightArray, leftArray.length);
	}
	return array;
}

function expand(inputArray, length) {
	if (length && length > inputArray.length) {
		const array = inputArray;
		inputArray = new Uint8Array(length);
		inputArray.set(array, 0);
	}
	return inputArray;
}