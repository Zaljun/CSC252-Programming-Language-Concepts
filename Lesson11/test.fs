open System;
open Microsoft.FSharp.Math;;
open System.Collections.Generic;;
open CSC7B;;    // compile with -r parser7e4.dll

// add the power operator
newoperator "\^" 700;; // need escape char \ ^ and | (not for &, !, =, etc)
newoperator "&" 250;   // these you will need for the assignment
newoperator "\|" 200;
newoperator "!" 300;
newoperator "=" 180;
binops <- List.append binops ["^";"|";"&";"="];;

// extend eval:
let eval0 = eval;
eval <- fun env exp ->
  match exp with
    | Binop("&",a,b) -> 
       if (eval env a) <> 0 && (eval env b) <> 0 then 1 else 0
    | Binop("|",a,b) -> 
       if (eval env a) = 0 && (eval env b) = 0 then 0 else 1
    | Binop("=",a,b) -> 
       if (eval env a) = (eval env b) then 1 else 0
    | Binop("^",a,b) ->
       let ea,eb = (eval env a), (eval env b)
       int(Math.Pow(float(ea),float(eb)))
    | Uniop("!",a) -> 
       if (eval env a) = 0 then 1 else 0
    | Ternop("if",a,b,c) ->
       if (eval env a) = 1 then (eval env b) else (eval env c)
    | e -> eval0 env e;;

// extend compile:  (compile <- ...)
let compile_b = compile
compile <- fun (exp,ln) ->
  match exp with
    | Binop("-",a,b) ->
        let (ca,ln2) = compile(a,ln)
        let (cb,ln3) = compile(b,ln2)
        let mutable ops = ca + cb
        ops <- addi(ops,"pop bx")
        ops <- addi(ops,"pop ax")
        ops <- addi(ops,"sub bx ax")
        ops <- addi(ops,"push ax")
        (ops,ln3+4)
    | Binop("*",a,b) ->
        let (ca,ln2) = compile(a,ln)
        let (cb,ln3) = compile(b,ln2)
        let mutable ops = ca + cb
        ops <- addi(ops,"pop bx")
        ops <- addi(ops,"pop ax")
        ops <- addi(ops,"imul bx ax")
        ops <- addi(ops,"push ax")
        (ops,ln3+4)
    | Binop("/",a,b) ->
        let (ca,ln2) = compile(a,ln)
        let (cb,ln3) = compile(b,ln2)
        let mutable ops = ca + cb
        ops <- addi(ops,"pop bx")
        ops <- addi(ops,"pop ax")
        ops <- addi(ops,"idiv bx ax")
        ops <- addi(ops,"push ax")
        (ops,ln3+4)
    | Binop("&",a,b) -> 
        let (ca,ln2) = compile(a,ln)
        let (cb,ln3) = compile(b,ln2)
        let mutable ops = ca+cb
        ops <- addi(ops,"pop bx")
        ops <- addi(ops,"pop cx")
        ops <- addi(ops,"imul bx cx")
        ops <- addi(ops,"jnz " + (ln3+5).ToString())         // jnz ln3+6
        ops <- addi(ops,"push 0")
        ops <- addi(ops,"jz "  + (ln3+7).ToString())        // jmp ln3+7
        ops <- addi(ops,"push 1")
        (ops,ln3+7)
    | Binop("|",a,b) ->            
        let (ca,ln2) = compile(a,ln)
        let (cb,ln3) = compile(b,ln2)
        let mutable ops = ca+cb
        ops <- addi(ops,"pop cx")
        ops <- addi(ops,"jnz " + (ln3+6).ToString())    // if a<>0 push 1
        ops <- addi(ops,"pop cx")                       // pop b
        ops <- addi(ops,"jnz " + (ln3+6).ToString())    // if b<>0 push 1
        ops <- addi(ops,"push 0")                       // if a,b = 0 push 0
        ops <- addi(ops,"jz "  + (ln3+7).ToString())    // jump out
        ops <- addi(ops,"push 1")
        (ops,ln3+7)
    | Binop("=",a,b) ->
        let (ca,ln2) = compile(a,ln)
        let (cb,ln3) = compile(b,ln2)
        let mutable ops = ca+cb
        ops <- addi(ops,"pop bx")
        ops <- addi(ops,"pop cx")
        ops <- addi(ops,"sub bx cx")
        ops <- addi(ops,"jnz " + (ln3+5).ToString())     // jnz ln3+6
        ops <- addi(ops,"push 1")
        ops <- addi(ops,"jz "  + (ln3+7).ToString())     // jmp l3+7
        ops <- addi(ops,"push 0")
        (ops,ln3+7)
    | Uniop("!",a) ->
        let (ca,ln2) = compile(a,ln)
        let mutable ops = ca
        ops <- addi(ops,"pop cx")
        ops <- addi(ops,"jnz " + (ln2+3).ToString())     // jnz ln2+4  
        ops <- addi(ops,"push 1")
        ops <- addi(ops,"jz "  + (ln2+5).ToString())     // jmp ln2+5
        ops <- addi(ops,"push 0")
        (ops,ln2+5)
    | Ternop("if",c,a,b) ->
        let (cc,ln2) = compile(c,ln)
        let (ca,ln3) = compile(a,ln2)
        let (cb,ln4) = compile(b,ln3)
        let mutable ops = cc+ca+cb
        ops <- addi(ops,"pop bx")
        ops <- addi(ops,"pop ax")
        ops <- addi(ops,"pop cx")
        ops <- addi(ops,"jnz " + (ln4+5).ToString())    // jnz ln4+6
        ops <- addi(ops,"push bx")
        ops <- addi(ops,"jz "  + (ln4+7).ToString())    // jmp ln4+7
        ops <- addi(ops,"push ax")
        (ops,ln4+7)
    | e -> compile_b (e,ln)


//advice_io(true,run);; // run with just console prompt and input advice
advice_io(true, fun () -> advice_trace(run));;  // run io + tracing
