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
    | [| "nop" |] -> NOP
    | z -> translate_b z;;

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

//run()
trace_advice( run );;

