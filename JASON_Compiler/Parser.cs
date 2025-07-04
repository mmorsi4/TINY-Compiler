﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JASON_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;

        /* MORSI BLOCK - DO NOT EDIT UNTIL END OF BLOCK */

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = Program();
            return root;
        }

        Node Program()
        {
            Node program = new Node("Program");

            program.Children.Add(Function_Statements());
            program.Children.Add(Main_Function());

            return program;
        }


        Node Function_Statements()
        {
            Node function_statements = new Node("Function_Statements");
            function_statements.Children.Add(Function_StatementsDash());
            return function_statements;
        }

        Node Function_StatementsDash()
        {
            Node function_statementsdash = new Node("Function_StatementsDash");

            // trace back this condition to the Datatype in Function_Declaration
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.T_INT || TokenStream[InputPointer].token_type == Token_Class.T_FLOAT || TokenStream[InputPointer].token_type == Token_Class.T_STRING))
            {
                function_statementsdash.Children.Add(Function_Statement());
                Function_StatementsDash_AUX(function_statementsdash);
            }

            return function_statementsdash;
        }

        void Function_StatementsDash_AUX(Node function_statementsdash)
        {
            // trace back this condition to the Datatype in Function_Declaration
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.T_INT || TokenStream[InputPointer].token_type == Token_Class.T_FLOAT || TokenStream[InputPointer].token_type == Token_Class.T_STRING))
            {
                function_statementsdash.Children.Add(Function_Statement());
                Function_StatementsDash_AUX(function_statementsdash);
            }
        }
        Node Function_Statement()
        {
            Node function_statement = new Node("Function_Statement");

            function_statement.Children.Add(Function_Declaration());
            function_statement.Children.Add(Function_Body());

            return function_statement;
        }
        Node Function_Declaration()
        {
            Node function_declaration = new Node("Function_Declaration");

            function_declaration.Children.Add(Datatype());
            function_declaration.Children.Add(FunctionName());
            function_declaration.Children.Add(match(Token_Class.T_LPAREN));
            function_declaration.Children.Add(ParameterList());
            function_declaration.Children.Add(match(Token_Class.T_RPAREN));

            return function_declaration;
        }

        Node ParameterList()
        {
            Node parameterlist = new Node("ParameterList");

            // trace back this condition to the Datatype in Parameter
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.T_INT || TokenStream[InputPointer].token_type == Token_Class.T_FLOAT || TokenStream[InputPointer].token_type == Token_Class.T_STRING))
            {
                parameterlist.Children.Add(Parameters());
            }

            return parameterlist;
        }

        Node Parameters()
        {
            Node parameters = new Node("Parameters");
            parameters.Children.Add(Parameter());
            Parameters_AUX(parameters);
            return parameters;
        }

        void Parameters_AUX(Node parameters)
        {
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.T_COMMA)
            {
                parameters.Children.Add(match(Token_Class.T_COMMA));
                parameters.Children.Add(Parameter());
                Parameters_AUX(parameters);
            }
        }

        Node Function_Body()
        {
            Node function_body = new Node("Function_Body");

            function_body.Children.Add(match(Token_Class.T_LCURLY));
            function_body.Children.Add(Statements());
            function_body.Children.Add(Return_Statement());
            function_body.Children.Add(match(Token_Class.T_RCURLY));

            return function_body;
        }
        Node Statements()
        {
            Node statements = new Node("Statements");
            statements.Children.Add(StatementsDash());
            return statements;
        }

        Node StatementsDash()
        {
            Node statementsdash = new Node("StatementsDash");

            if (InputPointer < TokenStream.Count &&
            (TokenStream[InputPointer].token_type == Token_Class.T_INT ||
            TokenStream[InputPointer].token_type == Token_Class.T_FLOAT ||
            TokenStream[InputPointer].token_type == Token_Class.T_STRING ||
            TokenStream[InputPointer].token_type == Token_Class.T_READ ||
            TokenStream[InputPointer].token_type == Token_Class.T_WRITE ||
            TokenStream[InputPointer].token_type == Token_Class.T_IF ||
            TokenStream[InputPointer].token_type == Token_Class.T_REPEAT ||
            TokenStream[InputPointer].token_type == Token_Class.T_IDENTIFIER))
            {
                statementsdash.Children.Add(Statement());
                StatementsDash_AUX(statementsdash);
            }

            return statementsdash;
        }

        void StatementsDash_AUX(Node statementsdash)
        {
            if (InputPointer < TokenStream.Count &&
            (TokenStream[InputPointer].token_type == Token_Class.T_INT ||
            TokenStream[InputPointer].token_type == Token_Class.T_FLOAT ||
            TokenStream[InputPointer].token_type == Token_Class.T_STRING ||
            TokenStream[InputPointer].token_type == Token_Class.T_READ ||
            TokenStream[InputPointer].token_type == Token_Class.T_WRITE ||
            TokenStream[InputPointer].token_type == Token_Class.T_IF ||
            TokenStream[InputPointer].token_type == Token_Class.T_REPEAT ||
            TokenStream[InputPointer].token_type == Token_Class.T_IDENTIFIER))
            {
                statementsdash.Children.Add(Statement());
                StatementsDash_AUX(statementsdash);
            }
        }

        Node Statement()
        {
            Node statement = new Node("Statement");

            if (InputPointer < TokenStream.Count)
            {
                // Declaration
                if (TokenStream[InputPointer].token_type == Token_Class.T_INT || TokenStream[InputPointer].token_type == Token_Class.T_FLOAT || TokenStream[InputPointer].token_type == Token_Class.T_STRING)
                {
                    statement.Children.Add(Declaration_Statement());
                }
                // Read
                else if (TokenStream[InputPointer].token_type == Token_Class.T_READ)
                {
                    statement.Children.Add(Read_Statement());
                }
                // Write
                else if (TokenStream[InputPointer].token_type == Token_Class.T_WRITE)
                {
                    statement.Children.Add(Write_Statement());
                }
                // If
                else if (TokenStream[InputPointer].token_type == Token_Class.T_IF)
                {
                    statement.Children.Add(If_Statement());
                }
                // Repeat
                else if (TokenStream[InputPointer].token_type == Token_Class.T_REPEAT)
                {
                    statement.Children.Add(Repeat_Statement());
                }
                // Function Call or Assignment
                else if (TokenStream[InputPointer].token_type == Token_Class.T_IDENTIFIER)
                {
                    if (InputPointer + 1 < TokenStream.Count)
                    {
                        // Function Call
                        if (TokenStream[InputPointer + 1].token_type == Token_Class.T_LPAREN)
                        {
                            statement.Children.Add(Function_Call());
                        }
                        // Assignment
                        else if (TokenStream[InputPointer + 1].token_type == Token_Class.T_ASSIGN)
                        {
                            statement.Children.Add(Assignment_Statement());
                        }
                        // Error
                        else
                        {
                            Errors.Error_List.Add("Parsing Error: Expected function call or assignment "
                             + " and " +
                            TokenStream[InputPointer].token_type.ToString() +
                            "  found\r\n");
                            InputPointer++;
                        }
                    }
                }
                // Error
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected statement "
                             + " and " +
                            TokenStream[InputPointer].token_type.ToString() +
                            "  found\r\n");
                    InputPointer++;
                }
            }

            return statement;
        }

        Node Main_Function()
        {
            Node main_function = new Node("Main_Function");

            main_function.Children.Add(Datatype());
            main_function.Children.Add(match(Token_Class.T_MAIN));
            main_function.Children.Add(match(Token_Class.T_LPAREN));
            main_function.Children.Add(match(Token_Class.T_RPAREN));
            main_function.Children.Add(Function_Body());

            return main_function;
        }

        /* END OF MORSI BLOCK */



        /* START OF YOUSSEF BLOCK*/

        Node If_Statement()
        {
            Node if_statement = new Node("If_Statement");

            if(InputPointer+4 < TokenStream.Count)
            {

                if (TokenStream[InputPointer].token_type == Token_Class.T_IF && TokenStream[InputPointer+4].token_type==Token_Class.T_END)
                {
                    if_statement.Children.Add(match(Token_Class.T_IF));
                    Unmatched(if_statement);


                }

                else if(TokenStream[InputPointer].token_type == Token_Class.T_IF&&(TokenStream[InputPointer].token_type == Token_Class.T_ELSEIF|| TokenStream[InputPointer].token_type == Token_Class.T_ELSE))
                {
                    if_statement.Children.Add(match(Token_Class.T_IF));
                    Matched(if_statement);
                }


                else
                {

                    Errors.Error_List.Add("Parsing Error: Expected if "
                            +"  found\r\n"+ TokenStream[InputPointer].token_type.ToString();
                    InputPointer++;

                }

            }

            else if(InputPointer  < TokenStream.Count & TokenStream[InputPointer].token_type == Token_Class.T_IF )
            {
                Errors.Error_List.Add("Parsing Error: Expected if statement but what was "
                           + "  found\r\n" + "incomplete if statement");
                InputPointer++;

            }

            return if_statement;


        }

        void Unmatched(Node if_statement)
        {

            if (InputPointer < TokenStream.Count)
            {

                if_statement.Children.Add(Condition_Statement());
                if_statement.Children.Add(match(Token_Class.T_THEN));
                if_statement.Children.Add(All_Statements());
                if_statement.Children.Add(match(Token_Class.T_END));
            }

            


        }


        void Matched(Node if_statement)
        {
            if (InputPointer < TokenStream.Count)
            {

                if_statement.Children.Add(Condition_Statement());
                if_statement.Children.Add(match(Token_Class.T_THEN));
                All_Statements(if_statement);
                Follow_Up(if_statement);
                if_statement.Children.Add(match(Token_Class.T_END));

            }
        }


        void All_Statements(Node if_statement)
        {
            if (InputPointer < TokenStream.Count)
            {
                if_statement.Children.Add(Statement());
                All_Statements_Dash(if_statement);


            }


        }

        void All_Statements_Dash(Node if_statement)
        {
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.T_INT || TokenStream[InputPointer].token_type == Token_Class.T_FLOAT || TokenStream[InputPointer].token_type == Token_Class.T_STRING
                    || TokenStream[InputPointer].token_type == Token_Class.T_READ|| TokenStream[InputPointer].token_type == Token_Class.T_WRITE
                    || TokenStream[InputPointer].token_type == Token_Class.T_IF|| TokenStream[InputPointer].token_type == Token_Class.T_REPEAT)
                {
                    if_statement.Children.Add(Statement());
                    All_Statements_Dash(if_statement);
                }
               
                else if (TokenStream[InputPointer].token_type == Token_Class.T_IDENTIFIER)
                {
                    if (InputPointer + 1 < TokenStream.Count)
                    {
                        // Function Call
                        if (TokenStream[InputPointer + 1].token_type == Token_Class.T_LPAREN|| (TokenStream[InputPointer + 1].token_type == Token_Class.T_ASSIGN)
                        {

                            if_statement.Children.Add(Statement());
                            All_Statements_Dash(if_statement);

                        }

                        else
                        {
                            Errors.Error_List.Add("Parsing Error: Expected function call or assignment "
                             + " and " +
                            TokenStream[InputPointer].token_type.ToString() +
                            "  found\r\n");
                            InputPointer++;
                        }
                    }
                }

            }


        }

        void Follow_Up(Node if_statement)
        {
            if (InputPointer < TokenStream.Count)
            {

                if (TokenStream[InputPointer].token_type == Token_Class.T_ELSEIF)
                {
                    if_statement.Children.Add(Else_If_Statement());
                }

                else if (TokenStream[InputPointer].token_type == Token_Class.T_ELSE)
                {
                    if_statement.Children.Add(Else_Statement());
                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected elseif or else "
                            + "  found\r\n" + TokenStream[InputPointer].token_type.ToString());
                    InputPointer++;

                }

            } 


        }

        Node Else_If_Statement()
        {
            Node else_if_statement = new Node("If_Statement");

            if (InputPointer < TokenStream.Count)
            {


                else_if_statement.Children.Add(match(Token_Class.T_ELSEIF));
                else_if_statement.Children.Add(Condition_Statement());
                else_if_statement.Children.Add(match(Token_Class.T_THEN));
                All_Statements(else_if_statement);

                if (TokenStream[InputPointer].token_type == Token_Class.T_ELSEIF)
                {
                    else_if_statement.Children.Add(Else_If_Statement());
                }

                else if (TokenStream[InputPointer].token_type == Token_Class.T_ELSE)
                {
                    else_if_statement.Children.Add(Else_Statement());
                }






            }




                return else_if_statement;

        }

        Node Else_Statement()
        {

            Node else_statement = new Node("If_Statement");
            else_statement.Children.Add(match(Token_Class.T_ELSE));
            All_Statements(else_statement);

            return else_statement;

        }




 



        /* END OF YOUSSEF BLOCK*/



        Node FunctionHeader()
        {
            Node header = new Node("Header");
            // write your code here to check the header sructure
            header.Children.Add(match(Token_Class.T_INT));
            //header.Children.Add(match(Token_Class.Idenifier));
            //header.Children.Add(match(Token_Class.Semicolon));
            return header;
        }
        /*
        Node VarDecl()
        {
            Node VarDecl = new Node("VarDecl");
            VarDecl.Children.Add(Datatype());
            VarDecl.Children.Add(Idlist());
            VarDecl.Children.Add(match(Token_Class.Semicolon));
            return VarDecl;
        }*/

        /*Node Datatype()
        {
            Node Datatype = new Node("Datatype");
            if (InputPointer < TokenStream.Count&&TokenStream[InputPointer].token_type == Token_Class.Integer ) {
                Datatype.Children.Add(match(Token_Class.Integer));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Real)
            {
                Datatype.Children.Add(match(Token_Class.Real));
            }
            else
            {   if (InputPointer < TokenStream.Count)
                {
                    Errors.Error_List.Add("Parsing Error: Expected integer or real "
                             + " and " +
                            TokenStream[InputPointer].token_type.ToString() +
                            "  found\r\n");
                    InputPointer++;
                }

            }
            return Datatype;
        }*/


        /*Node VarDecls()
        {
            Node VarDecls = new Node("VarDecls");
            VarDecls.Children.Add(VarDecl());
            VDecls(VarDecls);
            
            return VarDecls;
        }
        /*
         vardecls
                   vardecl
                   vardecl
         
         */

        /*
        void VDecls(Node VarDecls )
        {
            
            if (InputPointer < TokenStream.Count &&( TokenStream[InputPointer].token_type == Token_Class.Integer|| TokenStream[InputPointer].token_type == Token_Class.Real))
            {
                VarDecls.Children.Add(VarDecl());
                VDecls(VarDecls);
            }
           
        }
        */

        /*

        Node Idlist()
        {
            Node Idlist = new Node("Idlist");
            Idlist.Children.Add(match(Token_Class.Idenifier));
            IdenList(Idlist);
            return Idlist;
        }
        */

        /*
        void IdenList(Node Idlist)
        {

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
              
                Idlist.Children.Add(match(Token_Class.Comma));
                Idlist.Children.Add(match(Token_Class.Idenifier));
                IdenList(Idlist);
                
            }

        }*/

        /*
        Node DeclSec()
        {
            Node declsec = new Node("DeclSec");
            // write your code here to check atleast the declare sturcure 
            // without adding procedures
            return declsec;
        }
        Node Block()
        {
            Node block = new Node("block");
            // write your code here to match statements
            return block;
        }*/

        // Implement your logic here

        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}