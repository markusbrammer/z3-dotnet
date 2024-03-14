#r "nuget: Microsoft.Z3, 4.11.2"

open Microsoft.Z3

let ctx = new Context()
let solver = ctx.MkSolver()

// ====================================================================================================================
// Function (mapping) def
// ====================================================================================================================

let f = ctx.MkFuncDecl("f", ctx.BoolSort, ctx.IntSort)

let b0 = ctx.MkBoolConst("b0")
let b1 = ctx.MkBoolConst("b1")
let b2 = ctx.MkBoolConst("b2")

let i0 = ctx.MkIntConst("i0")
let i1 = ctx.MkIntConst("i1")

solver.Reset()

solver.Add(
    ctx.MkAnd(
        [| ctx.MkNot(ctx.MkEq(i0, i1))
           ctx.MkEq(f.Apply(b0), i0)
           ctx.MkEq(f.Apply(b1), i0)
           ctx.MkEq(f.Apply(b2), i1) |]
    )
)

solver.Check(ctx.MkNot(ctx.MkEq(b0, b1))) // unsat
