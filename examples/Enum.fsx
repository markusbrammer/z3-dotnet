#r "nuget: Microsoft.Z3, 4.11.2"

open Microsoft.Z3

let ctx = new Context()
let solver = ctx.MkSolver()

// ----------------------------------------------------------------------------
// EnumSort and case/const expressions

let fruitSort = ctx.MkEnumSort("Fruit", [| "Apple"; "Banana"; "Orange" |])

// The input for the EnumSort.Const(<integer>) function is uint32: 
// Either use `uint32 <integer>` or `<integer>u`.
let appleExpr = fruitSort.Const(0u)
let bananaExpr = fruitSort.Const(1u)
let orangeExpr = fruitSort.Const(2u)

// ----------------------------------------------------------------------------
// Equality for EnumSort

let f1 = ctx.MkFreshConst("f1", fruitSort)
solver.Reset()
solver.Add(ctx.MkEq(f1, appleExpr))
solver.Check()
solver.Model.Eval(f1).ToString() // "Apple"
solver.Reset()

