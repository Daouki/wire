﻿using WireC.AST.Statements;

namespace WireC.AST
{
    public interface IStatementVisitor
    {
        void VisitFunctionDefinition(FunctionDefinition functionDefinition);
        void VisitReturnStatement(ReturnStatement returnStatement);
        void VisitVariableDefinition(VariableDefinition variableDefinition);
        void VisitAssertStatement(AssertStatement assertStatement);
        void VisitIfStatement(IfStatement ifStatement);
        void VisitExpressionStatement(ExpressionStatement expressionStatement);
        void VisitWhileStatement(WhileStatement whileStatement);
        void VisitAssignmentStatement(AssignmentStatement assignmentStatement);
        void VisitBreakStatement(BreakStatement breakStatement);
        void VisitContinueStatement(ContinueStatement continueStatement);
    }
}