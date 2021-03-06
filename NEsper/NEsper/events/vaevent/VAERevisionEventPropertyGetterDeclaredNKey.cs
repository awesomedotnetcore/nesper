///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2017 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.client;
using com.espertech.esper.codegen.core;
using com.espertech.esper.codegen.model.expression;
using com.espertech.esper.collection;

using static com.espertech.esper.codegen.model.expression.CodegenExpressionBuilder;

namespace com.espertech.esper.events.vaevent
{
    public class VAERevisionEventPropertyGetterDeclaredNKey : EventPropertyGetterSPI
    {
        private readonly int _keyPropertyNumber;

        public VAERevisionEventPropertyGetterDeclaredNKey(int keyPropertyNumber)
        {
            _keyPropertyNumber = keyPropertyNumber;
        }

        public object Get(EventBean eventBean)
        {
            var riv = (RevisionEventBeanDeclared) eventBean;
            var key = (MultiKeyUntyped) riv.Key;
            if (key == null) return null;
            return key.Keys[_keyPropertyNumber];
        }

        public bool IsExistsProperty(EventBean eventBean)
        {
            return true;
        }

        public object GetFragment(EventBean eventBean)
        {
            return null;
        }

        public ICodegenExpression CodegenEventBeanGet(ICodegenExpression beanExpression, ICodegenContext context)
        {
            return LocalMethod(GetCodegen(context), beanExpression);
        }

        public ICodegenExpression CodegenEventBeanExists(ICodegenExpression beanExpression, ICodegenContext context)
        {
            return ConstantTrue();
        }

        public ICodegenExpression CodegenEventBeanFragment(ICodegenExpression beanExpression, ICodegenContext context)
        {
            return ConstantNull();
        }

        public ICodegenExpression CodegenUnderlyingGet(ICodegenExpression underlyingExpression, ICodegenContext context)
        {
            throw VAERevisionEventPropertyGetterDeclaredGetVersioned.RevisionImplementationNotProvided();
        }

        public ICodegenExpression CodegenUnderlyingExists(ICodegenExpression underlyingExpression,
            ICodegenContext context)
        {
            throw VAERevisionEventPropertyGetterDeclaredGetVersioned.RevisionImplementationNotProvided();
        }

        public ICodegenExpression CodegenUnderlyingFragment(ICodegenExpression underlyingExpression,
            ICodegenContext context)
        {
            throw VAERevisionEventPropertyGetterDeclaredGetVersioned.RevisionImplementationNotProvided();
        }

        private string GetCodegen(ICodegenContext context)
        {
            return context.AddMethod(typeof(object), typeof(EventBean), "eventBean", GetType())
                .DeclareVar(typeof(RevisionEventBeanDeclared), "riv",
                    Cast(typeof(RevisionEventBeanDeclared), Ref("eventBean")))
                .DeclareVar(typeof(MultiKeyUntyped), "key",
                    Cast(typeof(MultiKeyUntyped),
                        ExprDotMethod(Ref("riv"), "GetKey")))
                .IfRefNullReturnNull("key")
                .MethodReturn(ArrayAtIndex(
                    ExprDotMethod(Ref("key"), "GetKeys"),
                    Constant(_keyPropertyNumber)));
        }
    }
} // end of namespace