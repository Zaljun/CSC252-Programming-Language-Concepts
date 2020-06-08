// shift-reduce parser and evaluater for online calculator, version 7E4 (2020)

(*  Ambiguous context-free grammer:

    E := var of string  |  
         val of float   |
         E + E          |
         E * E          |
         E - E          |
         E / E          |
         (E);;
         - E;           // unary minus - reduce-reduce precedence
                        //( will have highest precedence - reduce, don't shift

    negative values will be handled by the tokenizer

    input stream of tokens will be represented as an array, from C# program
    global index will point to the next token.

    parse stack will be a list of expressions, starting with empty stack.
    left-side is tos so read parse stack backwards.

    Changes since original version: code reorganized, abstract syntax
    expanded to allow future extensions. eval procedure takes a
    Dictionary<string,int> as binding environment for the evaluation
    of variables.  Some procedures became mutables so AOP style
    "advice" can be written modularly.

    Ternary Expressions of the form   if (x) a else b   that should
    evaluate a if x is non-zero and evaluate b if x is zero.

    Note: the lexical analyzer is not capable of distinguishing between
    "=" and "==" as separate tokens.  Use words like "le" for <=, etc,
    instead of symbols that can be misinterpreted.
*)

module CSC7B
open System;;
open Microsoft.FSharp.Math;;
open System.Text.RegularExpressions;;
open System.Collections.Generic;;

///////////////// Lexical Symbol Specification

// use regular expression to represent possible operator symbols:
let mutable operators = "([()\+\-\*/:;,]|\s|\[|\]|=)";;
let mutable keywords = ["if";"else";"while";"let";"eq";"printnum";"printchr";"begin";"end"];
// keywords not currently used.

// use hash table (Dictionary) to associate each operator with precedence
let prectable = Dictionary<string,int>();;
prectable.["+"] <- 200;
prectable.["-"] <- 300;
prectable.["*"] <- 400;
prectable.["/"] <- 500;
prectable.["("] <- 990;
prectable.[")"] <- 20;
prectable.[":"] <- 20;
prectable.["="] <- 20;

// function to add new operator (as regex string) with precedence (int)
let newoperator (s:string) prec =
  let n = operators.Length
  let prefix = operators.Substring(0,n-1)
  operators <- prefix + "|" + s + ")"
  if s.[0]='\\' then prectable.[s.Substring(1,s.Length-1)] <- prec
  else prectable.[s] <- prec;;

//sample usage of newoperator function:
//newoperator @"&&" 650;;  // use @ before string or use "\^" (explict escape)
//Console.WriteLine(string(prectable.["&&"]));;  // check if success

// newoperator "^" 600;;   // @"^" didn't work - don't know why

// need newoperator "\?" precedence level
// need newoperator "%" precedence level

/// Lexical Token stream and abstract syntax combined into expr


////////////////////  Abstract Syntax (not all cases used in this program) ///

type expr = | Val of int | Binop of (string*expr*expr) | Uniop of (string*expr) | Var of string | Ternop of string*expr*expr*expr | Seq of (expr list) | Sym of string | EOF ;;

//// Note: because it's difficult to extend a discriminated union modularly,
// we are using strings to represent different kinds of expressions, so there
// is a cost to be paid in terms of static type safety.  Although pure F# is
// statically "type safe", there is not much type information available if
// use strings to represent data.  Instead, part of "type checking" has to
// be done at runtime with code like the following:

let mutable binops = ["+";"*";"/";"-";"%";"^";"while"];
let mutable unaryops = ["-"; "!"; "~"];
let mutable ternaryops = ["let"; "if"];;

// Proper expression check (shallow): separates proper expression from tokens
// This is the price to pay for using strings: no compile-time verification
let mutable proper = fun f ->
  match f with
    | Binop(s,_,_) when (List.exists (fun x->x=s) binops)  -> true
    | Uniop(s,_) when (List.exists (fun x->x=s) unaryops) -> true
    | Ternop("let",Var(x),_,_) -> true
    | Ternop("if",a,b,c) -> true    
    | Ternop(s,_,_,_) when (List.exists (fun x->x=s) ternaryops) -> true
    | Binop(_,_,_) -> false;
    | Uniop(_,_) -> false;
    | Sym(_) -> false
    | EOF -> false
    | _ -> true;;

// Because of variable expressions, env represents bindings for variables.
// eval is mutable so we can inject behaviors later...
// mutable funs can't be recursive calls, unless we declare eval first:
let mutable eval = fun (env:Dictionary<string,int>) (exp:expr) -> 0;;
eval <- fun (env:Dictionary<string,int>) exp ->
  match exp with
    | Val(v) -> v
    | Binop("+",a,b) -> (eval env a) + (eval env b)  // not Plus(a,b)
    | Binop("*",a,b) -> (eval env a) * (eval env b)  // lose some static safety
    | Binop("-",a,b) -> (eval env a) - (eval env b)
    | Binop("/",a,b) -> (eval env a) / (eval env b)
    | Uniop("-",a) -> -1 * (eval env a)
    | Ternop("let",Var(x),e1,e2) ->
       env.[x] <- (eval env e1)   // bind x to value, store in env
       eval env e2
    | Var(s) -> env.[s]
    | Seq(s) ->  // evaluate sequence (list) of expressions
      let mutable ax = 0  // default result 
      for e in s do (ax <- eval env e)
      ax  // return value of last expression in sequence
//    | Seq([e]) -> eval env e  // version without for loop
//    | Seq(a::b::c) ->
//        ignore (eval env a) // ignore required to ignore return value
//        eval env (Seq(b::c))
    | x -> raise (Exception("unrecognized eval case: "+string(x)));;
////////////////////////////////////////////////////

///////////////////////////////////////////////////
//Base Compiler

let mutable compile = fun (exp:expr,line:int) -> ("",0) // dummy
// returns next line number, initial line number should be 0
let addi(s:string,inst) = s+inst+"\n"; // for readability

//compiler take as arguments: symbol table (environ), expression, next line num.
compile <- fun (exp,ln) ->
   match exp with
    | Val(x) -> (("push "+string(x)+"\n"), ln+1)
    | Binop("+",a,b) ->
        let (ca,ln2) = compile(a,ln)
        let (cb,ln3) = compile(b,ln2)
        let mutable ops = ca+cb   // concatenate ca and cb strings into ops
        ops <- addi(ops,"pop bx") // pops result of b into bx
        ops <- addi(ops,"pop ax") // pops result of a into ax
        ops <- addi(ops,"add bx ax")
        ops <- addi(ops,"push ax")
        (ops,ln3+4)  // return value ln3+4 more instructions
    | x -> raise(Exception("compilation of "+string(x)+" not supported"));;


//////////////         LEXICAL ANALYSER (LEXER)  // (ignore most here)

// Reads expressions like  "7+3*2" and convert to list of symbols like
// [|Val(7.0);Sym("+");Val(3.0);Sym("*");Val(2.0);EOF|];
//// This is the first stage of parsing, called lexical analysis or token
// analysis

// assume string is in inp

// now build list of tokens
let maketoken x =  try Val(int(x))   // exception handling in F#
                   with
                   | excp ->
                      match x with
                       | y when (List.contains y keywords) -> Sym(y)
                       | y when int(y.[0])>96 && int(y.[0])<123 ->  Var(y)
                       | y -> Sym(y);;

let tokenize (s2:string[]) = 
  let rec itokenize ax = function   // inner tail-recursive function
    | i when i>=0 -> 
       let t = s2.[i].Trim()
       if (t<>"") then 
         itokenize (maketoken(s2.[i])::ax) (i-1)
       else
         itokenize ax (i-1)
    | _ -> ax;
  itokenize [EOF] (s2.Length-1);;

let mutable TS =[];;
let mutable TI = 0;; // global index for TS stream;;

let mutable inp = "";; // default input string

let lexer(inp:string) =
  let s2 = Regex.Split(inp,operators)
  TS <- tokenize s2
  TI <- 0  // reset if needed
  printfn "token stream: %A" TS;;


///////////////////


///////////////////
////////////////////////// SHIFT-REDUCE PARSER ////////////////////////

// function defines precedence of symbol, which includs more than just Syms
let mutable precedence = fun s ->
  match s with
   | Val(_) -> 100
   | Var(_) -> 100
   | Sym("if") -> 100
   | Sym("else") -> 100
   | Sym(s) when prectable.ContainsKey(s) -> prectable.[s]
   | EOF    -> 10
   | _ -> 11;;

// 3-2-1  (3-2)-1  left associativity

// Function defines associativity: true if left associative, false if right...
let mutable leftassoc = fun e ->
  match e with
   | _ -> true;  // most operators are left associative.

// Not all operators are left-associative: the assignment operator is
// right associative:  a = b = c; means first assign c to b, then b to a,
// as is the F# type operator ->: a->b->c means a->(b->c).

// check for precedence, associativity, and proper expressions to determine
// if a reduce rule is applicable.
let mutable checkreducible = fun (a,b,e1,e2) ->
  let (pa,pb) = (precedence(a),precedence(b))
  ((a=b && leftassoc(a)) || pa>=pb) && proper(e1) && proper(e2);;

// parse takes parse stack and lookahead; default is shift

////////////////// HERE IS THE HEART OF THE SHIFT-REDUCE PARSER ////////
let mutable parse = fun (x:expr list,expr) -> EOF // dummy for recursion
parse <- fun (stack,lookahead) ->
  match (stack,lookahead) with
   | ([e],EOF) when proper(e) -> e   // base case, returns an expression
   | (Sym(")")::e1::Sym("(")::t, la) when checkreducible(Sym("("),la,e1,e1) ->
        parse (e1::t,la)
   | (e2::Sym(op)::e1::cdr,la)   // generic case for binary operators
     when (List.exists (fun x->x=op) binops) && checkreducible(Sym(op),la,e1,e2) ->
        let e = Binop(op,e1,e2)
        parse(e::cdr,la)
   | (e1::Sym(uop)::t, la)
     when (List.exists (fun x->x=uop) unaryops) && checkreducible(Sym(uop),la,e1,e1) ->
        let e = Uniop(uop,e1)
        parse (e::t,la)
   | (e3::Sym("else")::e2::e1::Sym("if")::t,la) // if-else already recognized**
     when checkreducible(Sym("else"),la,e1,e2) && proper(e3) -> 
        let e = Ternop("if",e1,e2,e3)
        parse(e::t,la)
   | (st,la) when (TI < TS.Length-1) ->  // shift case
        TI <- TI+1;         
        let newla = TS.[TI]
        parse (la::st,newla)
   | (st,la) ->
        raise (Exception("parsing error: "+string(la::st)));;
/////////////////////////////////


////// AOP-style "advice" to trace parse, eval functions
let mutable advice_trace = fun (target:unit->unit) ->
  let proceed_parse = parse   // simulate dynamic scoping
  let proceed_eval = eval     // "crosscut" parse and eval
  let mutable cx = 1; // counter to prevent recursive advice on eval:
  parse <- fun(st,la) ->
     Console.WriteLine("parsing "+string(st)+" with lookahead "+string(la))
     proceed_parse(st,la)
  eval <- fun env e ->
     if (cx>0) then Console.Write("evaluating "+string(e));
     cx <- 0  // do not trace again for recursive calls
     proceed_eval env e
  target()  // execute target operation
  eval <- proceed_eval      
  parse <- proceed_parse;; // restore originals before exit
//advice_trace

////// Advice to handle exceptions gracefully
let mutable advice_errhandle = fun (target:unit->unit) ->
  let proceed_parse = parse
  let proceed_eval = eval
  parse <- fun(st,la) ->
     try proceed_parse(st,la) with
       | exc ->
           Console.WriteLine("parse failed with exception "+string(exc))
           exit(1)
  eval <- fun env e ->
     try (proceed_eval env e) with
       | exc ->
           Console.Write("eval failed with exception "+string(exc))
           Console.WriteLine(", returning 0 as default...")
           0
  target()  // execute target
  eval <- proceed_eval      
  parse <- proceed_parse;; // restore originals before exit
// advice_errhandle

/// Note that these advice functions group together code that "crosscut" 
// the conventional function-oriented design to be oriented instead towards
// certain "aspects" of the program (tracing, error handling).

///// Advice to read from stdin with prompt:
let mutable advice_io = fun (prompt, target:unit->unit) ->
    if prompt then Console.Write("Enter expression to be evaluated: ");
    inp <- Console.ReadLine()
    lexer(inp)
    target();;
//advice_io doesn't need to modify any functions, just inject before target


//// main execution function
let mutable run = fun () ->
  let ee = parse([],TS.[0])
  let mutable Bindings = Dictionary<string,int>() 
  // insert bindings
  // Bindings.["x"] <- 2
  let v = eval Bindings ee
  printf "\nValue of %s is %d\n" inp v;
  try  // try to compile if possible (does nothing if exception thrown)
     let (code,ln) = compile(ee,0)
     printfn "compiled code:\n%snop" code
  with
   | _ -> ();;


//////// RUN UNDER ADVICE, innermost advice will have precedence:
// advice_io(true, fun () -> advice_errhandle( fun () -> advice_trace( run ) ));; 

//lexer(Console.ReadLine()); // tokenize without prompt
// run();; // runs without any advice, must call lexer on some input string.


// default:  (commented out when generating .dll)
//advice_io(true,run);; // run with just console prompt and input advice
//advice_io(true, fun () -> advice_trace(run));;
