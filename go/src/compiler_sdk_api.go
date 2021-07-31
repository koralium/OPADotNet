package main

import "C"
import (
	"encoding/json"
	"sync"

	"github.com/open-policy-agent/opa/ast"
)

var compilerMutex sync.Mutex
var compilerToken int
var compilerMap sync.Map

//export RemoveCompiler
func RemoveCompiler(compilerId int) {
	compilerMap.Delete(compilerId)
}

func getCompiler(id int) *ast.Compiler {
	compiler, ok := compilerMap.Load(id)

	if !ok {
		panic("could not find compiler")
	}
	return compiler.(*ast.Compiler)
}

//export CompilePolicy
func CompilePolicy(fileName *C.char, rawText *C.char) C.int {
	fileNameGo := C.GoString(fileName)
	rawTextGo := C.GoString(rawText)

	module, err := compilePolicy(fileNameGo, rawTextGo)

	if err != nil {
		return addError(err.Error())
	}

	bytes, err := json.Marshal(module)

	if err != nil {
		return addError(err.Error())
	}

	return addString(string(bytes))
}

//export CompileModules
func CompileModules(modules *C.char) C.int {
	modulesJson := C.GoString(modules)

	var moduleData map[string]string
	err := json.Unmarshal([]byte(modulesJson), &moduleData)

	if err != nil {
		return addError(err.Error())
	}

	compiler, err := compileModules(moduleData)

	if err != nil {
		return addError(err.Error())
	}

	compilerMutex.Lock()
	compilerToken++
	i := compilerToken
	compilerMutex.Unlock()
	compilerMap.Store(i, compiler)
	return C.int(i)
}

//export GetModules
func GetModules(compilerId C.int) C.int {
	compiler := getCompiler(int(compilerId))
	modules := getModules(compiler)

	bytes, err := json.Marshal(modules)

	if err != nil {
		return addError(err.Error())
	}

	return addString(string(bytes))
}
