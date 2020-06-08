module CSC7C
(*    CSC 7B/FC Sample Program and Assignment (2019 version)

   Abstract Machine AM7B is a simple computer with 4K memory that can
   only be used as a stack.  The machine's CPU contains four general
   purpose registers, ax, bx, cx and dx, special purpose register sp
   (top of stack), an program counter register pc.  Word size is 32
   bits (so there are 1024 memory locations). The ALU can perform
   operations add, sub, imul and idiv.  The semantics of an
   instruction such as  (sub ax bx)  is bx -= ax: the second operand is
   also the destination operand.  There is an exception with the integer
   division instruction: (idiv ax bx) will leave the quotient in bx and
   remainder in dx.

   The only other operations (currently) are push and pop. Operands
   can be register or immediate, and only the push operation can take
   an immediate (constant) operand.

   For example, the following program calculates 5+4*3:

    +
  /   \ 
 5     *
      / \
     4   3
     
   push 3
   push 4
   pop ax
   pop bx
   imul bx ax
   push 5
   pop bx
   add bx ax
   push ax    // leave result on top of stack

AM7B programs should always leave the final result on top of the stack.

This program implements A FRAGMENT OF AM7B in F#, including assembler.
Your assignment would be to complete the implementation.
*)

open System;
open System.Collections.Generic;;
open Microsoft.FSharp.Math;
open System.Text.RegularExpressions;;

// operands are either immediate or register  (Mem is for future expansion)
type operand = Imm of int | Reg of string | Mem of operand;;

// updated for Spring 2020: instructions JMP, CALL, RET and NOP added:
type operation = ALU of (string*operand*operand) | PUSH of operand | POP of operand | MOV of operand*operand | JNZ of int | JZ of int | JN of int | JMP of int | CALL of int | RET | NOP;;


// an operation such as 'add ax bx' is represented internally as 
// ALU("add",Reg("ax"),Reg("bx"))
// A program represented by a list of such instructions, 
// which can be pattern matched.

// Currently the program only implements PUSH, POP and ALU operations, but
// the others are listed for later expansion (I will address how they can
// added modularly later, which would require 'active patterns').


////////////////////// Machine Simulation //////////////////////

let RAM:int[] = Array.zeroCreate 1024;  // 4K memory, only used for stack
let mutable sp = 0;;                 // top of stack register (addresses RAM)
// sp points to next available slot on stack, so 0 means empty stack.
let mutable pc = 0;;                 // program counter register
let mutable fault = false;;          // machine fault flag
let REGS = Dictionary<string,int>();;  // simulate general registers
REGS.["ax"] <- 0   // initially all 0's
REGS.["bx"] <- 0
REGS.["cx"] <- 0
REGS.["dx"] <- 0;;
//REGS.["pc"] <- 0     // these registers should be outside of software control
//REGS.["sp"] <- 0     // so don't add to this table

// but this means exception thrown might not be what you want...
let getreg name =
   try REGS.[name]
   with
     | :? KeyNotFoundException as ke ->
        raise (Exception("no such register: "+name));
let setreg(name,v) =
   try (REGS.[name] <- v)
   with
     | :? KeyNotFoundException as ke ->
        raise (Exception("no such register: "+name));;


// ALU operations dispatch to lambda terms:
let mutable aluop = fun n ->
  match n with
    | "add" -> fun (x,y) -> y+x
    | "sub" -> fun (x,y) -> y-x  
    | "imul" -> fun (x,y) -> y*x
    | "idiv" -> fun (x,y) -> y/x
    | "rem" -> fun (x,y) -> x % y
    | x   -> raise(Exception("illegal ALU operation: "+x));;

(*  The above function maps alu operations "add", "sub", etc to
    lambda terms that perform the corresponding calculuations.  There are
    two things to note here:
     1.  We could have also used a Dictionary<string,int*int->int>, but
         it would again be harder to control exceptions.
     2.  Unlike previous functions, this function is written as a mutable
         lambda term, so it can be changed.  That is, we can assign
	 'aluop' to another lambda term.  Why would we want to do that?
	 Because we may want to modify it later if we want to add additional
	 instructions, like this:

  let base_aluop = aluop  // save copy of original before changing it
  aluop <- fun n ->   // assign aluop to new lambda term
    match n with
      | "cmp" -> fun(x,y) -> if (x=y) then 1 else 0 // compare operation
      | z -> base_aluop z;;  // differ to original function otherwise

Now aluop recognizes one more operation, plus the original ones.  But we
did this without having to edit the original code. Since aluop is a global
definition, all functions that call aluop are also changed.  This is
basically simulating the dynamic scoping of aluop (see more extensive
example at the end).
*)

// core dump prints current snapshot of registers plus top portion of stack.
let coredump() =
  printfn "MACHINE FAULT, CORE DUMPED, YOU SUCK!"
  printfn "ax=%d, bx=%d, cx=%d, dx=%d" (REGS.["ax"]) (REGS.["bx"]) (REGS.["cx"]) (REGS.["dx"])
  printfn "sp=%d,  top portion of stack:\n" sp
  let mutable i = sp-1
  while (i>=0 && i>sp-8) do
     printfn "%d" (RAM.[i])
     i <- i-1;;
     

///////// *** KEY FUNCTION *** ...
// The following implements some of the operations of AM7B, but not all...

// Execute a single operation - note execute is also a mutable lambda function
let mutable execute = fun instruction ->
 match instruction with
  | ALU(op,Reg(a),Reg(b)) when op<>"idiv" ->  // can't handle idiv yet
      let f = aluop(op)  // get function 
      REGS.[b] <- f(REGS.[a],REGS.[b])
  | ALU(op,Imm(a),Reg(b)) when op<>"idiv" ->
      let f = aluop(op)
      REGS.[b] <- f(a,REGS.[b])
  | ALU(op,Reg(a),Reg(b)) when op = "idiv" ->
      let f = aluop(op)
      let g = aluop("rem")
      REGS.[b] <- f(REGS.[a],REGS.[b])
      REGS.["dx"] <- f(REGS.[a],REGS.[b])
  | PUSH(Imm(x)) when sp<RAM.Length ->  // push immediate, as in push 3
      RAM.[sp] <- x
      sp <- sp+1
  | PUSH(Reg(r)) when sp<RAM.Length ->  // push register r contents
      RAM.[sp] <- REGS.[r]
      sp <- sp+1
  | POP(Reg(r)) when sp>0 ->            // pop into register r
      sp <- sp - 1
      REGS.[r] <- RAM.[sp] 
  | MOV(Imm(x),Reg(b)) -> 
      REGS.[b] <- x
  | MOV(Reg(a),Reg(b)) ->
      REGS.[b] <- REGS.[a]
  | MOV(Reg(a),Mem(Reg(b))) -> 
      RAM.[REGS.[b]] <- REGS.[a]
  | MOV(Mem(Reg(b)),Reg(a)) -> 
      REGS.[a] <- RAM.[REGS.[b]]
  | JNZ(x) -> 
      if REGS.["cx"]<>0 then pc <- x-1
  | JZ(x) ->
      if REGS.["cx"]=0 then pc <-x-1
  | JMP(x) ->
      pc <- x-1
  | NOP -> ()   // do nothing for nop   (added for 2020 version)
  | x -> (fault<-true; Console.WriteLine("fault on "+string(x)); coredump());;


//Execute a program as a list of operations. Note how pc is incremented
//so any jump instruction must be written carefully.
let mutable executeProg = fun (program:operation list) ->
  pc <- 0
  fault <- false
  while not(fault) && pc<program.Length do
    let instruction = program.[pc];
    execute instruction
    pc <- pc+1
   //end of while by indentation
  if not(fault) then printfn "top of stack holds value %d" (RAM.[sp-1]);

let rec optmize = fun orig opt->
  let mutable optmz: operation list = opt
  match orig with
  | (PUSH(Reg(a))::POP(Reg(b))::c) when a<>b ->
    optmz <- (MOV(Reg(a),Reg(b)))::optmz
    optmize c optmz
  | (PUSH(Reg(a))::POP(Reg(b))::c) when a=b ->
    //optmz <- optmz
    optmize c optmz
  | (MOV(Reg(a),Reg(b))::MOV(Reg(x),Reg(y))::c) when a=y && b=x ->
    optmz <- (MOV(Reg(a),Reg(b)))::optmz
    optmize c optmz
  | ins::c -> 
    optmz <- ins::optmz
    optmize c optmz
  | [] -> optmz

let executeProg_b = executeProg
executeProg <- fun (program:operation list) ->
  let opmtz = optmize program []
  executeProg_b opmtz
//let rec executeProg = function //(program: operation list) ->
 ////match program with
  //| (PUSH(Reg(a))::POP(Reg(b))::c) when a<>b -> 
    //  execute (MOV(Reg(a),Reg(b)))
    //  pc <- pc+2
    //  executeProg c
  //| (PUSH(Reg(a))::POP(Reg(b))::c) when a=b ->
    //  execute NOP
    //  pc <- pc+2
    //  executeProg c
  //| (MOV(Reg(a),Reg(b))::MOV(Reg(e),Reg(d))::c) when a=d && b=e ->
    //  execute (MOV(Reg(a),Reg(b)))
    //  pc <- pc+2
    //  executeProg c
  //| [] -> printfn "top of stack holds value %d" (RAM.[sp-1])
  //| (ins::c) -> 
    //  //let instr = program.[pc]
    //  //execute instr
    //  execute ins
    //  pc <- pc+1
    //  executeProg c

//let executeProg = function (program: operation list) ->
  //pc <-0
  //fault <- false
  // //let mutable instruction = program.[pc]
  //while not(fault) && pc<program.Length do
    //match program with
     //| (PUSH(Reg(a))::POP(Reg(b))::c) when a<>b -> 
       // execute (MOV(Reg(a),Reg(b)))
       //pc <- pc+2
     //| (PUSH(Reg(a))::POP(Reg(b))::c) when a=b ->
       // execute NOP
       // pc <- pc+2
     //| (MOV(Reg(a),Reg(b))::MOV(Reg(e),Reg(d))::c) when a=d && b=e ->
       // execute (MOV(Reg(a),Reg(b)))
       // pc <- pc+2     
     //| _ -> 
       // let instr = program.[pc]
        //execute instr
        //pc <- pc+1
  //if not(fault) then printfn "top of stack holds value %d" (RAM.[sp-1]);

let executeProg2 = function (program: operation list) ->
  pc <-0
  fault <- false
  //let mutable instruction = program.[pc]
  while not(fault) && pc<program.Length-1 do
    match (program.[pc]::program.[pc+1]::[]) with
     | (PUSH(Reg(a))::POP(Reg(b))::[]) when a<>b -> 
        execute (MOV(Reg(a),Reg(b)))
        pc <- pc+2
     | (PUSH(Reg(a))::POP(Reg(b))::[]) when a=b ->
        execute NOP
        pc <- pc+2
     | (MOV(Reg(a),Reg(b))::MOV(Reg(e),Reg(d))::[]) when a=d && b=e ->
        execute (MOV(Reg(a),Reg(b)))
        pc <- pc+2     
     | _ -> 
        let instr = program.[pc]
        execute instr
        pc <- pc+1
  execute program.[pc]
  if not(fault) then printfn "top of stack holds value %d" (RAM.[sp-1]);
      
  

///////////////////////////// ASSEMBLER/SIMULATION /////////////////////////
// This part of the code is fairly mundane: it reads text such as "add ax bx"
// from stdin and converts them in to a list of AM7B operations.

//let ins = Console.ReadLine(); // read instruction into string format
let ins = "add ax bx"
let tokens = Regex.Split(ins,"\s"); // split string by white spaces
// tokens is of type string[], a .Net array of Strings


// [| "add"; "ax"; "bx" |]

// translate a string into an operand:  (note mutable lambda)
let mutable transoperand = fun x -> 
  try Imm(int(x))    // try-with block  exception handling
  with
  | exce when x = "[bx]" -> Mem(Reg("bx"))
  //| x when x.[0]='[' && x.[x.Length-1]=']' && x.Substring(1,2)="bx" -> Mem(Reg("bx"))
  | exce when x.[0]='[' && x.[x.Length-1]=']' && x.Substring(1,2)<>"bx" ->
       raise (Exception("illegal memory operand"))
  | exce -> Reg(x);;   // operand is immediate or memory, but so far
                       // memory operand [bx] not recognized.

// translate a string token array into an AM7B instruction
let mutable translate = fun ary ->   // another mutable lambda
  match ary with
    | [| "push"; x |] -> PUSH(transoperand x)
    | [| "pop"; x |] -> POP(transoperand x)
    | [| "mov"; x; y|] -> MOV(transoperand x, transoperand y) //
    | [| "jnz"; x|] -> JNZ(int(x)) //
    | [| "jz"; x|] -> JZ(int(x)) //
    | [| "jmp"; x|] -> JMP(int(x)) //
    | [| op; x; y |] -> ALU(op, transoperand x, transoperand y)
    | i -> raise(Exception("Unrecognized instruction: "+string(i)));

let assemble i = translate (Regex.Split(i,"\s"));;

// test that it works
let i1 = assemble "add ax bx";
let i2 = assemble "push 3";
let i3 = assemble "pop cx";
//Console.WriteLine("---");

/// read am7b program from stdin, assemble and execute:
let rec readprog ax (ins:string) =
  match ins with
    | null -> ax
    | n when n.Length=0 || n.[0] = '#'  -> // skip blank or comment line
       readprog ax (Console.ReadLine())
    | ist ->
       let inst = ist.Trim()    
       readprog (assemble(inst)::ax) (Console.ReadLine());;

let rec reverse ax = function
  | [] -> ax
  | (a::b) -> reverse (a::ax) b;;

//******* MAIN FUNCTION THAT RUNS ON EXECUTION OF THE PROGRAM:
// read file from stdin, assemble and execute:
let run() =   
  let firstline = Console.ReadLine();
  let instructions = reverse [] (readprog [] firstline);
  executeProg(instructions) // better if run under advice:


// When we run a program we want to have the option of tracing.
// we can do this by temporarily modifying the execute function to
// print the instruction being executed as well as a snapshot of the
// registers.  We can do this in the form of an "advice", which is
// something that *injects* extra code into existing code, a concept from
// AOP (aspect oriented programming).  We can write code that temporarily
// changes the execute function by simulating *dynamic scoping* of the function.

// AOP style code enhancement with simulated dynamic scoping of functions
let trace_advice (target:unit->unit) =
   let storefun = execute // save execute function on stack
   execute <- fun inst ->
     try 
       Console.WriteLine("executing instruction: "+string(inst))
       storefun inst  // run original execute
       printfn "ax=%d bx=%d cx=%d dx=%d sp=%d pc=%d" (REGS.["ax"]) (REGS.["bx"]) (REGS.["cx"]) (REGS.["dx"]) sp (pc+1)
     with
       | :? KeyNotFoundException as ke ->
          Console.Write("no such register: ")
          Console.WriteLine(ke)
       | exc -> (Console.WriteLine(exc); exit(1)) // catch exception
   target();  // execute target code under local dynamic scope
   execute <- storefun;;   // exit dynamic scope, restore original function

// reset machine counters before running program
pc <- 0;
sp <- 0;
//run();   // run without any advice
// Run with (mono) machine.exe < test1.7b

//run();;
// **Comment out following line when compiling to .dll: fsharpc --target:library
// or fsc /t:library   :
trace_advice( run );;   // execute target function 'run' under advice



