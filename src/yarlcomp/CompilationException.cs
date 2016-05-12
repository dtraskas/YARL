// Copyright (c) Dimitrios Traskas. All rights reserved. See README.md in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace DSparx.YARL
{    
    internal class CompilationException : ApplicationException
    {
        public CompilationException()
        {
            
        }
        public CompilationException(string message)
            : base(message)
        {
            
        }
        public CompilationException(string message, Exception innerException)
            : base(message, innerException)
        {
            
        }
        protected CompilationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }        
    }
}
