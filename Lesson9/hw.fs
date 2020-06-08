(*   EXERCISE I1 - I5
I1.*** EXERCISE I1: 
> let S = fun x y z -> x(z)(y(z));;

*** EXERCISE I2:
> printfn "%d %d" 1 (g(2,4));;

*** EXERCISE I3: 
> let rec f(x) = 
   if x>0 then
     printfn "%d" x
     f(x-1)
   else printfn "%d" x;;
OR
> let f x =
   let mutable y = x
   while y > 0 do
     printfn "%d" y
     y <- y-1;;

*** EXERCISE I4: 
> let x = 1;
  let f() = x;
  let m() = 
    let x = 2;
    printfn "%d" (f());;
> m();;
  1
// Thus it is static.

*** EXERCISE I5: 
> let a = [|1;2;3|];;
> let f (x: 'a[]) = 
    x.[0] <- 0
    x;;
> f a;;
> a;;
// a changes to [|0;2;3|], which means it is passed by a pointer
*)

//    EXERCISE C1 - C3
// ***EXERCISE C1 
open System;;

type expr = Num of int | Plus of expr*expr | Times of expr*expr | Neg of expr;;


let t = Plus(Num(2),Times(Num(4),Neg(Num(3))));;  // (2 + 4 * -3)
let d = Plus(Num(5),Neg(Num(2)));;                // (5 + -2)
let c = Plus(Num(5),Neg(Neg(Neg(Neg(Num(2))))));;           // (5 + --2)

let rec eval = function
   | Num(n) -> n
   | Plus(a,b) -> eval(a) + eval(b)
   | Times(a,b) -> eval(a) * eval(b)
   | Neg(a) -> eval(a) * -1;;


let rec tostring = function
   | Num(n) -> string(n)
   | Plus(a,Neg(Neg(Neg(x)))) -> "(" + tostring(a) + " - " + tostring(Neg(Neg(x))) + ")"
   // modified Plus: a + --b = a + b
   | Plus(a,Neg(Neg(x))) -> "(" + tostring(a) + " + " + tostring(Neg(Neg(x))) + ")"    
   // modified Plus: a + -b  = a - b
   | Plus(a,Neg(x)) -> "(" + tostring(a) + " - " + tostring(x) + ")" 
   | Plus(a,b) -> "(" + tostring(a) + " + " + tostring(b) + ")"  
   | Times(a,b) -> tostring(a) + "*" + tostring(b)
   | Neg(Neg(x)) -> tostring(x)    
   | Neg(x) -> "-" + tostring(x);;

System.Console.WriteLine(tostring(t));;         // (2 + 4 * -3)
System.Console.WriteLine(tostring(d));;         // (5 + -2)
System.Console.WriteLine(tostring(c));;         // (5 + --2)
// Output Sample:
// (2 + 4*-3)
// (5 - 2)
// (5 + 2)

type 'a stack = Emp | Apnd of 'a stack * 'a;; // 'Apnd' for "append"

let s = Apnd(Apnd(Apnd(Emp,2),3),5);; //instead of Cons(2,Cons(3,Cons(5,Emp)));
let r = 1::(2::(3::[]));;

let pushon a stk = Apnd(stk,a);;
let popoff = function
  | Emp -> raise (System.Exception("stack underflow"))
  | Apnd(s,a) -> a;;

let rec tolist2 ax = function               // stack to list
  | Emp -> ax    // ax is the accumulator
  | Apnd(s,a) -> tolist2 (a::ax) s;;

// ***EXERCISE C2: 
let rec sz = function                       // size of a stack
  | Emp -> 0
  | Apnd(s,a) -> 1 + sz s;;

// ***EXERCISE C3: 
let rec tostk (ax: 'a stack) = function     // list to stack
  | a::[] -> ax
  | a::l -> tostk (Apnd(ax,a)) l;;

let sl = tolist2 [] s;;                     // stack to list
let size = sz s;;                           // size = size of stack s

let ls = tostk Emp r;;                      // list to stack

System.Console.WriteLine(size);;            // test size 
System.Console.WriteLine(sl);;
System.Console.WriteLine(ls);;              // test list to stack
// Output Sample:
// 3
// [2; 3; 5]
// Hw+stack`1+Apnd[System.Int32]
