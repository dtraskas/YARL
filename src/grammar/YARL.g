/**
* 
*	YARL GRAMMAR
*	Dimitrios Traskas
*	
*	
**/

grammar YARL;

options {
	language=CSharp3;
	TokenLabelType=CommonToken;
	output=AST;
	ASTLabelType=CommonTree;
}

tokens
{
	PROGRAM;
	FUZZYVAR;
	RULEDEF;
	RULESETDEF;
	EXECUTEDEF;
	FUZZYRANGEDEF;
	FUZZYSETDEF;
	FUZZYSETSHAPEDEF;
	SHAPES;
	SHAPEP;
	SHAPEZ;
	DESCDEF;
	CERTAINTYDEF;
	PRIORITYDEF;
	WEIGHTDEF;
	WHENDEF;
	ANTECEDENTSDEF;
	CONSEQUENTDEF;
	OPERATORDEF;
	VARIABLEDEF;
	ANTECEDENTDEF;
	RULESETCHOICEDEF;
	RULECOLLECTIONDEF;
	RULECOLLECTIONDEFCHOICE;
	RULECOLLECTIONALLDEFCHOICE;
	CONTAINSALL;
}

@lexer::namespace {DSparx.YARL.Compiler}

@parser::namespace{DSparx.YARL.Compiler}

@rulecatch {
   catch (RecognitionException e) {
    throw e;
   }
}

@parser::members {
  public override void ReportError(RecognitionException e) {
    throw e;
  }
}


public program
	: (fuzzyvarDef | ruleDef | rulesetDef | executeDef)+
	  -> ^(PROGRAM fuzzyvarDef+ ruleDef+ rulesetDef+ executeDef+)
	;
	
	
/************* MAIN DEFINITIONS ****************************/

fuzzyvarDef
	: 'fuzzyvar' ID (fuzzyrangeDef | descDef)*
	 'begin' 
	 fuzzysetDef*
	 'end'
	 -> ^(FUZZYVAR ID fuzzyrangeDef* descDef* fuzzysetDef*)
	;

ruleDef	
    	: 'rule' ID 
    	  (certaintyDecl |
       	   priorityDecl |
   	  	   descDef |
	  	   weightDecl)*
	  	   whenStatement  	      
    	  'end'
    	  -> ^(RULEDEF ID certaintyDecl* priorityDecl* descDef* weightDecl* whenStatement)
    	;

rulesetDef 
	: 'ruleset' ID 
	   descDef+
	   'contains' rulesetChoiceDef+
   	   -> ^(RULESETDEF ID descDef+ rulesetChoiceDef+)
	;	

rulesetChoiceDef
	: ruleCollectionDef -> ^(RULECOLLECTIONDEFCHOICE ruleCollectionDef)
	 | 'all' -> ^(RULECOLLECTIONALLDEFCHOICE CONTAINSALL)
	;

ruleCollectionDef
	: '(' ID ((',') ID)* ')'
	-> ^(RULECOLLECTIONDEF ID*)
	;

executeDef
	: 'execute' '(' ID ((',') ID)* ')'
  	   -> ^(EXECUTEDEF ID*)
	;


/************* FUZZY VARIABLE DEFINITIONS ****************************/

fuzzyrangeDef
	: 'range' '(' number ',' number ')' 	
	-> ^(FUZZYRANGEDEF number*)
	;
	
fuzzysetDef
	: fuzzysetShapeDef+
	  '(' 
	  ID ',' number ',' number ','* number*
	  ')' 
  	  -> ^(FUZZYSETDEF fuzzysetShapeDef+ ID number*)
	;

fuzzysetShapeDef
	: (
	     'set_\\' -> ^(SHAPES)  
	   | 'set_/\\' -> ^(SHAPEP)
	   | 'set_/' -> ^(SHAPEZ)
	   )
	;

/************* RULE DEFINITIONS ****************************/	

descDef
	: 'desc' '(' STRING_LITERAL ')'	
	-> ^(DESCDEF STRING_LITERAL)
	;
	
certaintyDecl
	: 'certainty' '(' number ')'
	-> ^(CERTAINTYDEF number)
	;

priorityDecl
	: 'priority' '(' number ')'
	-> ^(PRIORITYDEF number)	
	;
	
weightDecl
	: 'weight' '(' number ')'
	-> ^(WEIGHTDEF number)
	;	

whenStatement
	: 'when' antecedents 'then' consequent
	-> ^(WHENDEF antecedents consequent)
	;	

consequent
	:	variable 'is' ID
	-> ^(CONSEQUENTDEF variable ID)
	;	

antecedents
	: antecedent (operator antecedent)*
	-> ^(ANTECEDENTSDEF antecedent* operator*)
	;

antecedent
	: '(' variable 'is' ID ')'
	-> ^(ANTECEDENTDEF variable ID)
	;


/************* BASIC DEFINITIONS ****************************/

operator
	:	'and' 	-> ^(OPERATORDEF 'and') 
	  | 'or' 	-> ^(OPERATORDEF 'or')
	;

variable
	: '$' ID
	-> ^(VARIABLEDEF ID)
	;

number 
	: FLOAT | INTEGER
	;	

MINUS: '-';

STRING_LITERAL
    :  '"' (ID | ' ')* '"'
    ;
    
CHAR_LITERAL
	:	'\'' . '\''
	;

ID  :	('a'..'z'|'A'..'Z'|'_') ('a'..'z'|'A'..'Z'|'0'..'9'|'_')*
    ;

INTEGER 
	:	('+'|'-')? '0'..'9'+
	;

FLOAT
    :   ('+'|'-')? 
    	('0'..'9')+ '.' ('0'..'9')* EXPONENT?
    |   '.' ('0'..'9')+ EXPONENT?
    |   ('0'..'9')+ EXPONENT
    ;

COMMENT
    :   '//' ~('\n'|'\r')* '\r'? '\n' {$channel=HIDDEN;}
    |   '/*' ( options {greedy=false;} : . )* '*/' {$channel=HIDDEN;}
    ;

WS  :   ( ' '
        | '\t'
        | '\r'
        | '\n'
        ) {$channel=HIDDEN;}
    ;

fragment
EXPONENT : ('e'|'E') ('+'|'-')? ('0'..'9')+ ;

