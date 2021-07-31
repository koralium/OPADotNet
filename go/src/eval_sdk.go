package main

import (
	"context"

	"github.com/open-policy-agent/opa/ast"
	"github.com/open-policy-agent/opa/rego"
	"github.com/open-policy-agent/opa/storage"
)

func prepareForEvaluation(compiler *ast.Compiler, store storage.Store, query string) (rego.PreparedEvalQuery, error) {
	ctx := context.Background()

	r := rego.New(
		rego.Query(query),
		rego.Compiler(compiler),
		rego.Store(store),
	)

	return r.PrepareForEval(ctx)
}

func preparedEval(prepared rego.PreparedEvalQuery, input interface{}) (rego.ResultSet, error) {
	ctx := context.Background()
	return prepared.Eval(ctx, rego.EvalInput(input))
}
