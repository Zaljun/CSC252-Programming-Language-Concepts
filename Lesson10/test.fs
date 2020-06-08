open System;
open CSC7B;;


let execute_b = execute
execute <- fun instruction ->
  match instruction with
    | ALU(op,Imm(a),Reg(b)) when op<>"idiv" ->
        let f = aluop(op)
        REGS.[b] <- f(a,REGS.[b])
    | ALU(op,Reg(a),Reg(b)) when op = "idiv" ->
        let f = aluop(op)
        REGS.[b] <- f(REGS.[a],REGS.[b])
        REGS.["dx"] <- (REGS.[b] % REGS.[a])    
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
    | z -> execute_b z;;

let transoperand_b = transoperand
transoperand <- fun x ->
  match x with
    | x when x = "[bx]" -> Mem(Reg("bx"))
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

let executeProg_b = executeProg
executeProg <- function (program: operation list) ->
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

//let rec optmize = fun orig opt->
  //let mutable optmz = opt
  //match orig with
  //| (PUSH(Reg(a))::POP(Reg(b))::c) when a<>b ->
    //optmz <- (MOV(Reg(a),Reg(b)))::optmz
    //optmize c optmz
  //| (PUSH(Reg(a))::POP(Reg(b))::c) when a=b ->
   // //optmz <- optmz
    //optmize c optmz
  //| (MOV(Reg(a),Reg(b))::MOV(Reg(x),Reg(y))::c) when a=y && b=x ->
    //optmz <- (MOV(Reg(a),Reg(b)))::optmz
    //optmize c optmz
  //| ins::c -> 
    //optmz <- ins::optmz
    //optmize c optmz
  //| [] -> optmz

//let executeProg_b = executeProg
//executeProg <- fun (program:operation list) ->
  //let optmz_r = reverse [] (optmize program [])
  //executeProg_b optmz_r

Console.Write("This is updated version!");;
trace_advice( run );;
Console.Write("This is updated version!!");;