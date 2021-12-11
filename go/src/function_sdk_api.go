package main

// #include <stdlib.h>
// typedef char* (*function_cb)(char*);
// char* callFunctionCallback(char* json, function_cb cb);
import "C"

//export RegisterFunction1
func RegisterFunction1(name *C.char, callback C.function_cb) {
	registerFunction1(C.GoString(name), callback)
}
