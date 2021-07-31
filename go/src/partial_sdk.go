package main

import (
	"context"

	"github.com/open-policy-agent/opa/ast"
	"github.com/open-policy-agent/opa/rego"
	"github.com/open-policy-agent/opa/storage"
)

func prepareForPartial(compiler *ast.Compiler, store storage.Store, query string) (rego.PreparedPartialQuery, error) {
	ctx := context.Background()

	r := rego.New(
		rego.Query(query),
		rego.Compiler(compiler),
		rego.Store(store),
	)

	return r.PrepareForPartial(ctx)
}

func preparedPartial(prepared rego.PreparedPartialQuery, input interface{}, unknowns []string) (*rego.PartialQueries, error) {
	ctx := context.Background()
	return prepared.Partial(ctx, rego.EvalInput(input), rego.EvalUnknowns(unknowns))
}
