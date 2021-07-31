package main

import (
	"github.com/open-policy-agent/opa/ast"
)

func compileModules(modules map[string]string) (*ast.Compiler, error) {
	return ast.CompileModules(modules)
}

// A function to help calling application to get a poliy in the AST structure
func compilePolicy(fileName string, rawText string) (*ast.Module, error) {
	compiler, err := ast.CompileModules(map[string]string{
		fileName: rawText,
	})

	if err != nil {
		return &ast.Module{}, err
	}

	return compiler.Modules[fileName], nil
}

func getModules(compiler *ast.Compiler) map[string]*ast.Module {
	return compiler.Modules
}
