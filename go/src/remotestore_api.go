package main

// #include <stdlib.h>
// typedef char* (*read_cb)(char*);
// char* callReadCallback(char* path, read_cb cb);
import "C"

//export RegisterRemoteStore
func RegisterRemoteStore(storeId C.int, name *C.char, callback C.read_cb) {
	store := getStore(int(storeId)).(*Store)
	store.registerRemoteStore(C.GoString(name), callback)
}
