grammar Calculator;

// Парсерні правила (як будуються вирази)
compileUnit: expr EOF;
expr: '(' expr ')'                         #Parentheses
    | expr op=('*' | '/') expr             #MultDiv
    | expr op=('+' | '-') expr             #AddSub
    | NUMBER                               #Number
    | CELL                                 #CellRef
    ;

// Лексерні правила (з яких символів складаються токени)
NUMBER: [0-9]+ ('.' [0-9]+)?;
CELL: [A-Z]+ [0-9]+;
WS: [ \t\r\n]+ -> skip;