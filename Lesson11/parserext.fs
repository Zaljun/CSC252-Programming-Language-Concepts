// Example of how to extend parser7e4.fs

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
//unaryops <- List.append unaryops ["!"];;   already in base program


// extend eval:
let eval0 = eval;
eval <- fun env exp ->
  match exp with
    | Binop("^",a,b) ->
       let ea,eb = (eval env a), (eval env b)
       int(Math.Pow(float(ea),float(eb)))
    | e -> eval0 env e;;

// extend compile:  (compile <- ...)


//advice_io(true,run);; // run with just console prompt and input advice
advice_io(true, fun () -> advice_trace(run));;  // run io + tracing
