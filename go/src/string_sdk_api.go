package main

//#include <stdlib.h>
import "C"

import (
	"sync"
	"unsafe"
)

var emptyString *C.char = C.CString("")

var stringMutex sync.Mutex
var stringToken int
var stringMap sync.Map
var errorMutex sync.Mutex
var errorToken int

func addString(txt string) C.int {
	stringMutex.Lock()
	stringToken++
	i := stringToken
	stringMutex.Unlock()
	stringMap.Store(i, C.CString(txt))
	return C.int(i)
}

func addError(txt string) C.int {
	errorMutex.Lock()
	errorToken--
	i := errorToken
	errorMutex.Unlock()
	stringMap.Store(i, C.CString(txt))
	return C.int(i)
}

//export FreeString
func FreeString(index C.int) {
	i := int(index)
	val, ok := stringMap.LoadAndDelete(i)

	if ok != true {
		return
	}

	str := val.(*C.char)

	C.free(unsafe.Pointer(str))
}

//export GetString
func GetString(index C.int) *C.char {
	i := int(index)
	val, ok := stringMap.Load(i)

	if ok != true {
		return emptyString
	}

	str := val.(*C.char)
	return str
}
