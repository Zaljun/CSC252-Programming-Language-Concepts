open System
open CSC7B

// PART I
let execute_b = execute
execute <- fun instruction ->
  match instruction with
    | ALU(op,Imm(a),Reg(b)) when op<>"idiv" ->   // add 2 ax
        let f = aluop(op)
        REGS.[b] <- f(a,REGS.[b])
    | ALU(op,Reg(a),Reg(b)) when op = "idiv" ->  // idiv ax bx
        let f = aluop(op)
        REGS.[b] <- f(REGS.[a],REGS.[b])
        REGS.["dx"] <- (REGS.[b] % REGS.[a])    
    | MOV(Imm(x),Reg(b)) ->                      // mov 2 ax
        REGS.[b] <- x
    | MOV(Reg(a),Reg(b)) ->                      // mov ax bx
        REGS.[b] <- REGS.[a]
    | MOV(Reg(a),Mem(Reg(b))) ->                 // mov ax [bx]
        RAM.[REGS.[b]] <- REGS.[a]
    | MOV(Mem(Reg(b)),Reg(a)) ->                 // mov [bx] ax
        REGS.[a] <- RAM.[REGS.[b]]
    | JNZ(x) ->                                  // jnz 2
        if REGS.["cx"]<>0 then pc <- x-1
    | JZ(x) ->                                   // jz 0
        if REGS.["cx"]=0 then pc <-x-1
    | JMP(x) ->                                  // jmp 1
        pc <- x-1
    | z -> execute_b z;;

let transoperand_b = transoperand
transoperand <- fun x ->
  match x with
    | x when x = "[bx]" -> Mem(Reg("bx"))
  //| x when x.[0]='[' && x.[x.Length-1]=']' && x.Substring(1,2)="bx" -> Mem(Reg("bx"))
    | x when x.[0]='[' && x.[x.Length-1]=']' && x.Substring(1,2)<>"bx" ->
        raise (Exception("illegal memory operand"))
    | z -> transoperand_b z;;


let translate_b = translate
translate <- fun ary ->
  match ary with
    | [| "mov"; x; y|] -> MOV(transoperand x, transoperand y)
    | [| "jnz"; x|] -> JNZ(int(x))
    | [| "jz"; x|] -> JZ(int(x))
    | [| "jmp"; x|] -> JMP(int(x))
    | z -> translate_b z;;


// For PART II, one method is to optimize the code then pass it to executeProg
// Other method is to rewrite executeProg, making it optimize the code when "compiling"

// Method 1 (optimize code)
let rec optimize = fun orig opt->
  let mutable optmz = opt
  match orig with
  | (PUSH(Reg(a))::POP(Reg(b))::c) when a<>b -> // push ax; pop bx = mov ax bx
    optmz <- (MOV(Reg(a),Reg(b)))::optmz
    optimize c optmz
  | (PUSH(Reg(a))::POP(Reg(b))::c) when a=b ->  // push ax; pop ax = nothing (eliminate)
    //optmz <- optmz
    optimize c optmz
  | (MOV(Reg(a),Reg(b))::MOV(Reg(x),Reg(y))::c) when a=y && b=x ->
    optmz <- (MOV(Reg(a),Reg(b)))::optmz        // mov ax bx; mov bx ax = mov ax bx
    optimize c optmz
  | ins::c -> 
    optmz <- ins::optmz
    optimize c optmz
  | [] -> optmz                                 // optimized but reversed

let executeProg_b = executeProg
executeProg <- fun (program:operation list) ->
  let optmz_r = reverse [] (optimize program [])
  executeProg_b optmz_r

// Method 2 (optimize executeProg, doesn't change codes)
(*
let executeProg_b = executeProg
executeProg <- function (program: operation list) ->
  pc <-0
  fault <- false
  while not(fault) && pc<program.Length-1 do
    match (program.[pc]::program.[pc+1]::[]) with
     | (PUSH(Reg(a))::POP(Reg(b))::[]) when a<>b -> 
        execute (MOV(Reg(a),Reg(b)))            // push ax; pop bx = mov ax bx
        pc <- pc+2
     | (PUSH(Reg(a))::POP(Reg(b))::[]) when a=b ->
        execute NOP                             // push ax; pop ax = nothing
        pc <- pc+2
     | (MOV(Reg(a),Reg(b))::MOV(Reg(e),Reg(d))::[]) when a=d && b=e ->
        execute (MOV(Reg(a),Reg(b)))            // mov ax bx; mov bx ax = mov ax bx
        pc <- pc+2     
     | _ -> 
        let instr = program.[pc]
        execute instr
        pc <- pc+1
  execute program.[pc]
  if not(fault) then printfn "top of stack holds value %d" (RAM.[sp-1]);
*)

//run()
trace_advice( run );;
Console.Write("This is updated version!");;

//////////////////////////////////////////////////
// Sample Output: 
(* 
test2.7b
未经处理的异常:  System.Exception: illegal memory operand   

test3.7b  
top of stack holds value 5
This is updated version!

test4.7b
top of stack holds value 720
This is updated version!

test7.7b:   push 5
            pop ax
            # mov ax bx
            push ax
            pop bx
            # eliminate (do nothing)
            push bx
            pop bx 
            push 2
            pop ax
            # mov ax bx
            mov ax bx
            mov bx ax
            push bx

Method 1
executing instruction: CSC7B+operation+PUSH
ax=0 bx=0 cx=0 dx=0 sp=1 pc=1
executing instruction: CSC7B+operation+POP
ax=5 bx=0 cx=0 dx=0 sp=0 pc=2
executing instruction: CSC7B+operation+MOV
ax=5 bx=5 cx=0 dx=0 sp=0 pc=3
executing instruction: CSC7B+operation+PUSH
ax=5 bx=5 cx=0 dx=0 sp=1 pc=4
executing instruction: CSC7B+operation+POP
ax=2 bx=5 cx=0 dx=0 sp=0 pc=5
executing instruction: CSC7B+operation+MOV
ax=2 bx=2 cx=0 dx=0 sp=0 pc=6
executing instruction: CSC7B+operation+PUSH
ax=2 bx=2 cx=0 dx=0 sp=1 pc=7
top of stack holds value 2
This is updated version!

Method 2
executing instruction: CSC7B+operation+PUSH
ax=0 bx=0 cx=0 dx=0 sp=1 pc=1
executing instruction: CSC7B+operation+POP
ax=5 bx=0 cx=0 dx=0 sp=0 pc=2
executing instruction: CSC7B+operation+MOV
ax=5 bx=5 cx=0 dx=0 sp=0 pc=3
executing instruction: CSC7B+operation
ax=5 bx=5 cx=0 dx=0 sp=0 pc=5
executing instruction: CSC7B+operation+PUSH
ax=5 bx=5 cx=0 dx=0 sp=1 pc=7
executing instruction: CSC7B+operation+POP
ax=2 bx=5 cx=0 dx=0 sp=0 pc=8
executing instruction: CSC7B+operation+MOV
ax=2 bx=2 cx=0 dx=0 sp=0 pc=9
executing instruction: CSC7B+operation+PUSH
ax=2 bx=2 cx=0 dx=0 sp=1 pc=11
top of stack holds value 2
This is updated version!
*)
