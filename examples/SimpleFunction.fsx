#r "nuget: Microsoft.Z3, 4.11.2"

open Microsoft.Z3

let ctx = new Context()
let solver = ctx.MkSolver()

// ====================================================================================================================
// Function def
// ====================================================================================================================

// Declare f(x) = x + 1
let f = ctx.MkFuncDecl("f", ctx.IntSort, ctx.IntSort)
let x = ctx.MkFreshConst("x", ctx.IntSort) :?> ArithExpr
solver.Add(ctx.MkForall([| x |], ctx.MkEq(f.Apply(x), ctx.MkAdd(x, ctx.MkInt(1)))))

// If f(y) = 3, what is y?
let y = ctx.MkFreshConst("y", ctx.IntSort) :?> ArithExpr
solver.Add(ctx.MkEq(f.Apply(y), ctx.MkInt(3)))
solver.Check()
solver.Model.Eval(y, true).ToString() // y = 2

// Can y be anything else?
solver.Check(ctx.MkNot(ctx.MkEq(y, ctx.MkInt(2)))) // Nope (unsat)

