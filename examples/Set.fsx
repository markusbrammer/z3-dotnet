#r "nuget: Microsoft.Z3, 4.11.2"

open Microsoft.Z3

let ctx = new Context()
let solver = ctx.MkSolver()

// ============================================================================
// Set
// ============================================================================

let mutable intSet = ctx.MkEmptySet(ctx.IntSort)
intSet <- ctx.MkSetAdd(intSet, ctx.MkInt(3))
intSet <- ctx.MkSetAdd(intSet, ctx.MkInt(2))

intSet.ToString()

solver.Reset()
solver.Check(ctx.MkSetMembership(ctx.MkInt(3), intSet)) // sat
solver.Check(ctx.MkSetMembership(ctx.MkInt(10), intSet)) // unsat

// ----------------------------------------------------------------------------
// Quantifiers

let mutable intSetNeg = ctx.MkEmptySet(ctx.IntSort)
intSetNeg <- ctx.MkSetAdd(intSetNeg, ctx.MkInt(-7))
intSetNeg <- ctx.MkSetAdd(intSetNeg, ctx.MkInt(-9))

let intSetSort = ctx.MkSetSort(ctx.IntSort)

// let test (xs: ArrayExpr) (ys: ArrayExpr) : BoolExpr = 
//     let x = ctx.MkFreshConst("x", ctx.IntSort)
//     let y = ctx.MkFreshConst("y", ctx.IntSort)
//
//     ctx.MkAnd([|
//         ctx.MkForall(
//             [| x |],
//             ctx.MkExists(
//                 [| y |],
//                 ctx.MkImplies(
//                     ctx.MkAnd(
//                         ctx.MkSetMembership(x, xs),
//                         ctx.MkSetMembership(y, ys)
//                     ),
//                     ctx.MkLe(x :?> ArithExpr, y :?> ArithExpr)
//                 )
//             )
//         ) :> BoolExpr;
//         ctx.MkNot(ctx.MkEq(xs, ctx.MkEmptySet(ctx.IntSort)));
//         ctx.MkNot(ctx.MkEq(ys, ctx.MkEmptySet(ctx.IntSort)))
//     |])
let test (xs: ArrayExpr) (ys: ArrayExpr) : BoolExpr = 
    let x = ctx.MkFreshConst("x", ctx.IntSort)
    let y = ctx.MkFreshConst("y", ctx.IntSort)

    ctx.MkAnd([|
        ctx.MkForall(
            [| x |],
            ctx.MkExists(
                [| y |],
                ctx.MkImplies(
                    ctx.MkAnd(
                        ctx.MkSetMembership(x, xs),
                        ctx.MkSetMembership(y, ys)
                    ),
                    ctx.MkLe(x :?> ArithExpr, y :?> ArithExpr)
                )
            )
        ) :> BoolExpr;
        ctx.MkNot(ctx.MkEq(xs, ctx.MkEmptySet(ctx.IntSort)));
        ctx.MkNot(ctx.MkEq(ys, ctx.MkEmptySet(ctx.IntSort)))
    |])



let zs : ArrayExpr = ctx.MkFreshConst("zs", intSetSort) :?> ArrayExpr

solver.Reset()
solver.Check(test intSet intSetNeg)
solver.Check(test intSet intSet)
solver.Check(test intSetNeg intSet)
solver.Check(test intSet zs)
solver.Model.Eval(zs, true).ToString()

solver.Reset()
solver.Check(test zs intSetNeg)
solver.Model.Eval(zs, true).ToString()

solver.Reset()
solver.Check(test intSetNeg zs)
solver.Model.Eval(zs, true).ToString()

// ----------------------------------------------------------------------------
// Mapping

let inc = ctx.MkFuncDecl("Inc", ctx.IntSort, ctx.IntSort)

let incRule : BoolExpr = 
    let i = ctx.MkFreshConst("i", ctx.IntSort)

    ctx.MkForall([| i |], ctx.MkEq(inc.Apply(i), ctx.MkAdd(i :?> ArithExpr, ctx.MkInt(1))))

solver.Reset()
solver.Add(incRule)
solver.Check(ctx.MkEq(inc.Apply(ctx.MkInt(1)), ctx.MkInt(2))) // sat
solver.Check(ctx.MkNot(ctx.MkEq(inc.Apply(ctx.MkInt(1)), ctx.MkInt(2)))) // unsat

