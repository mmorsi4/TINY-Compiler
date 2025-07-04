﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

public enum Token_Class
{
    T_NUMBER, T_STRING, T_IDENTIFIER,

    T_INT, T_FLOAT, T_STRING_RESERVED, T_READ, T_WRITE, T_REPEAT, T_UNTIL,
    T_IF, T_ELSEIF, T_ELSE, T_THEN, T_RETURN, T_ENDL, T_MAIN,T_END,

    T_COMMENT,

    T_PLUS, T_MINUS, T_MULTIPLY, T_DIVIDE, T_ASSIGN, T_LESS_THAN, T_GREATER_THAN,
    T_EQUAL, T_NOT_EQUAL, T_AND, T_OR,

    T_LPAREN, T_RPAREN, T_LCURLY, T_RCURLY, T_SEMICOLON, T_COMMA
}

namespace JASON_Compiler
{


    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Symbols = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("int", Token_Class.T_INT);
            ReservedWords.Add("float", Token_Class.T_FLOAT);
            ReservedWords.Add("string", Token_Class.T_STRING_RESERVED);
            ReservedWords.Add("read", Token_Class.T_READ);
            ReservedWords.Add("write", Token_Class.T_WRITE);
            ReservedWords.Add("repeat", Token_Class.T_REPEAT);
            ReservedWords.Add("until", Token_Class.T_UNTIL);
            ReservedWords.Add("if", Token_Class.T_IF);
            ReservedWords.Add("elseif", Token_Class.T_ELSEIF);
            ReservedWords.Add("else", Token_Class.T_ELSE);
            ReservedWords.Add("then", Token_Class.T_THEN);
            ReservedWords.Add("return", Token_Class.T_RETURN);
            ReservedWords.Add("endl", Token_Class.T_ENDL);
            ReservedWords.Add("end", Token_Class.T_END);
            ReservedWords.Add("main", Token_Class.T_MAIN);

            Operators.Add("+", Token_Class.T_PLUS);
            Operators.Add("-", Token_Class.T_MINUS);
            Operators.Add("*", Token_Class.T_MULTIPLY);
            Operators.Add("/", Token_Class.T_DIVIDE);
            Operators.Add(":=", Token_Class.T_ASSIGN);
            Operators.Add("<", Token_Class.T_LESS_THAN);
            Operators.Add(">", Token_Class.T_GREATER_THAN);
            Operators.Add("=", Token_Class.T_EQUAL);
            Operators.Add("<>", Token_Class.T_NOT_EQUAL);
            Operators.Add("&&", Token_Class.T_AND);
            Operators.Add("||", Token_Class.T_OR);

            Symbols.Add("(", Token_Class.T_LPAREN);
            Symbols.Add(")", Token_Class.T_RPAREN);
            Symbols.Add("{", Token_Class.T_LCURLY);
            Symbols.Add("}", Token_Class.T_RCURLY);
            Symbols.Add(";", Token_Class.T_SEMICOLON);
            Symbols.Add(",", Token_Class.T_COMMA);
        }


        public void StartScanning(string SourceCode)

        {


            Console.WriteLine("Console attached. Now logging works!");
            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n' || CurrentChar == '\t')
                    continue;

                if (CurrentChar >= 'A' && CurrentChar <= 'z') //if you read a character
                {
                    j++;
                    while (j < SourceCode.Length && !(SourceCode[j] == ' ' || SourceCode[j] == '\r' || SourceCode[j] == '\n' || SourceCode[j] == '\t' ||
                         (j + 1 < SourceCode.Length && SourceCode[j] == '|' && SourceCode[j + 1] == '|') ||
                         (j + 1 < SourceCode.Length && SourceCode[j] == '&' && SourceCode[j + 1] == '&') ||
                         (SourceCode[j] == ';') || (j + 1 < SourceCode.Length && SourceCode[j] == ':' && SourceCode[j + 1] == '=') || Symbols.ContainsKey(SourceCode[j].ToString()) || Operators.ContainsKey(SourceCode[j].ToString())))
                    {

                        CurrentLexeme += SourceCode[j];
                        j++;

                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;

                }

                else if (CurrentChar >= '0' && CurrentChar <= '9')
                {
                    j++;
                    while (j < SourceCode.Length && !(SourceCode[j] == ' ' || SourceCode[j] == '\r' || SourceCode[j] == '\n' || SourceCode[j] == '\t' ||
                        (j + 1 < SourceCode.Length && SourceCode[j] == '|' && SourceCode[j + 1] == '|') ||
                        (j + 1 < SourceCode.Length && SourceCode[j] == '&' && SourceCode[j + 1] == '&') ||
                        (SourceCode[j] == ';') || (j + 1 < SourceCode.Length && SourceCode[j] == ':' && SourceCode[j + 1] == '=') || Symbols.ContainsKey(SourceCode[j].ToString()) || Operators.ContainsKey(SourceCode[j].ToString())))
                    {

                        CurrentLexeme += SourceCode[j];
                        j++;

                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                }


                else if (CurrentChar == '"')
                {
                    j++;
                    while (j < SourceCode.Length && SourceCode[j] != '"')
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;

                    }
                    if (j < SourceCode.Length)
                    {
                        CurrentLexeme += SourceCode[j];
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j;
                }



                else if (CurrentChar == '/' && j + 1 < SourceCode.Length && (SourceCode[j + 1] == '*'))
                {
                    j += 2;

                    while (j < SourceCode.Length && j + 1 < SourceCode.Length)
                    {
                        if (SourceCode[j] == '*' && SourceCode[j + 1] == '/')
                        {
                            j++;
                            break;
                        }
                        j++;

                    }

                    i = j;
                }


                else if (Operators.ContainsKey(CurrentLexeme) || Symbols.ContainsKey(CurrentLexeme) || CurrentChar == ':' || CurrentChar == '&' || CurrentChar == '|')
                {

                    if (CurrentChar == '<' && j != (SourceCode.Length - 1))
                    {
                        if (SourceCode[j + 1] == '>')
                        {
                            j++;
                            CurrentLexeme += SourceCode[j];
                        }
                    }

                    else if (CurrentChar == ':' && j != (SourceCode.Length - 1))
                    {
                        if (SourceCode[j + 1] == '=')
                        {
                            j++;
                            CurrentLexeme += SourceCode[j];
                        }
                    }



                    else if (CurrentChar == '&' && j != (SourceCode.Length - 1))
                    {
                        if (SourceCode[j + 1] == '&')
                        {
                            j++;
                            CurrentLexeme += SourceCode[j];
                        }
                    }

                    else if (CurrentChar == '|' && j != (SourceCode.Length - 1))
                    {
                        if (SourceCode[j + 1] == '|')
                        {
                            j++;
                            CurrentLexeme += SourceCode[j];
                        }
                    }

                    FindTokenClass(CurrentLexeme);
                    i = j;
                }



                else
                { // Collect invalid lexime until we find a delimiter

                    Debug.WriteLine("what");

                    j++;

                    while (j < SourceCode.Length && !(SourceCode[j] == ' ' || SourceCode[j] == '\r' || SourceCode[j] == '\n'))
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }

                    FindTokenClass(CurrentLexeme);
                    Debug.WriteLine(CurrentLexeme);
                    i = j - 1;
                }
            }

            JASON_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;

            //Is it a reserved word?
            if (ReservedWords.ContainsKey(Lex))
            {
                TC = ReservedWords[Lex];
                Tok.token_type = TC;

                Tokens.Add(Tok);
            }

            //Is it an identifier?
            else if (isIdentifier(Lex))
            {
                TC = Token_Class.T_IDENTIFIER;
                Tok.token_type = TC;

                Tokens.Add(Tok);

            }

            //Is it a number??
            else if (isNumber(Lex))
            {
                TC = Token_Class.T_NUMBER;
                Tok.token_type = TC;

                Tokens.Add(Tok);
            }

            //Is it a string??
            else if (isString(Lex))
            {
                TC = Token_Class.T_STRING;
                Tok.token_type = TC;

                Tokens.Add(Tok);
            }

            //Is it an operator?
            else if (Operators.ContainsKey(Lex))
            {
                Tok.token_type = Operators[Lex];
                if (Tok.token_type.Equals(":="))
                {

                }
                Tokens.Add(Tok);
            }

            //Is it a symbol?
            else if (Symbols.ContainsKey(Lex))
            {
                Tok.token_type = Symbols[Lex];
                Tokens.Add(Tok);
            }

            //Is it an undefined?
            else
            {
                Errors.Error_List.Add(Lex);
            }

        }



        bool isIdentifier(string lex)
        {
            bool isValid = true;
            // Check if the lex is an identifier or not.
            var rx = new Regex(@"^[a-zA-Z][0-9a-zA-Z]*$");
            if (!rx.IsMatch(lex))
            {
                isValid = false;
            }
            return isValid;
        }

        bool isString(string lex)
        {
            bool isValid = true;

            // Check if the lex is a constant (Number) or not.
            Regex str = new Regex(@"^\""[^\""]*\""$");

            if (!str.IsMatch(lex)) isValid = false;

            return isValid;
        }

        bool isNumber(string lex)
        {
            bool isValid = true;

            // Check if the lex is a constant (Number) or not.
            Regex number = new Regex(@"^([1-9][0-9]*(\.[0-9]+)?|0(\.[0-9]+)?)$");

            if (!number.IsMatch(lex)) isValid = false;

            return isValid;
        }
    }
}