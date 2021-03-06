// Copyright (c) 2010-2014 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

//------------------------------------------------------------------------------
// <auto-generated>
//     Types declaration for CorApi.Portable.Win32 namespace.
//     This code was generated by a tool.
//     Date : 11/05/2017 11:23:36
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Runtime.InteropServices;
using System.Security;
#if true
namespace CorApi.Portable.Win32 {

#pragma warning disable 282
#pragma warning disable 649
#pragma warning disable 419
#pragma warning disable 1587
#pragma warning disable 1574
    
    /// <summary>	
    /// No documentation.	
    /// </summary>	
    /// <include file='..\..\Documentation\CodeComments.xml' path="/comments/comment[@id='STATSTG']/*"/>	
    /// <unmanaged>STATSTG</unmanaged>	
    /// <unmanaged-short>STATSTG</unmanaged-short>	
    public  partial struct StorageStatistics {	
        
        /// <summary>	
        /// No documentation.	
        /// </summary>	
        /// <include file='..\..\Documentation\CodeComments.xml' path="/comments/comment[@id='STATSTG::pwcsName']/*"/>	
        /// <unmanaged>wchar_t* pwcsName</unmanaged>	
        /// <unmanaged-short>wchar_t pwcsName</unmanaged-short>	
        public string PwcsName;
        
        /// <summary>	
        /// No documentation.	
        /// </summary>	
        /// <include file='..\..\Documentation\CodeComments.xml' path="/comments/comment[@id='STATSTG::type']/*"/>	
        /// <unmanaged>unsigned int type</unmanaged>	
        /// <unmanaged-short>unsigned int type</unmanaged-short>	
        public int Type;
        
        /// <summary>	
        /// No documentation.	
        /// </summary>	
        /// <include file='..\..\Documentation\CodeComments.xml' path="/comments/comment[@id='STATSTG::cbSize']/*"/>	
        /// <unmanaged>ULARGE_INTEGER cbSize</unmanaged>	
        /// <unmanaged-short>ULARGE_INTEGER cbSize</unmanaged-short>	
        public long CbSize;
        
        /// <summary>	
        /// No documentation.	
        /// </summary>	
        /// <include file='..\..\Documentation\CodeComments.xml' path="/comments/comment[@id='STATSTG::mtime']/*"/>	
        /// <unmanaged>FILETIME mtime</unmanaged>	
        /// <unmanaged-short>FILETIME mtime</unmanaged-short>	
        public long Mtime;
        
        /// <summary>	
        /// No documentation.	
        /// </summary>	
        /// <include file='..\..\Documentation\CodeComments.xml' path="/comments/comment[@id='STATSTG::ctime']/*"/>	
        /// <unmanaged>FILETIME ctime</unmanaged>	
        /// <unmanaged-short>FILETIME ctime</unmanaged-short>	
        public long Ctime;
        
        /// <summary>	
        /// No documentation.	
        /// </summary>	
        /// <include file='..\..\Documentation\CodeComments.xml' path="/comments/comment[@id='STATSTG::atime']/*"/>	
        /// <unmanaged>FILETIME atime</unmanaged>	
        /// <unmanaged-short>FILETIME atime</unmanaged-short>	
        public long Atime;
        
        /// <summary>	
        /// No documentation.	
        /// </summary>	
        /// <include file='..\..\Documentation\CodeComments.xml' path="/comments/comment[@id='STATSTG::grfMode']/*"/>	
        /// <unmanaged>unsigned int grfMode</unmanaged>	
        /// <unmanaged-short>unsigned int grfMode</unmanaged-short>	
        public int GrfMode;
        
        /// <summary>	
        /// No documentation.	
        /// </summary>	
        /// <include file='..\..\Documentation\CodeComments.xml' path="/comments/comment[@id='STATSTG::grfLocksSupported']/*"/>	
        /// <unmanaged>unsigned int grfLocksSupported</unmanaged>	
        /// <unmanaged-short>unsigned int grfLocksSupported</unmanaged-short>	
        public int GrfLocksSupported;
        
        /// <summary>	
        /// No documentation.	
        /// </summary>	
        /// <include file='..\..\Documentation\CodeComments.xml' path="/comments/comment[@id='STATSTG::clsid']/*"/>	
        /// <unmanaged>GUID clsid</unmanaged>	
        /// <unmanaged-short>GUID clsid</unmanaged-short>	
        public System.Guid Clsid;
        
        /// <summary>	
        /// No documentation.	
        /// </summary>	
        /// <include file='..\..\Documentation\CodeComments.xml' path="/comments/comment[@id='STATSTG::grfStateBits']/*"/>	
        /// <unmanaged>unsigned int grfStateBits</unmanaged>	
        /// <unmanaged-short>unsigned int grfStateBits</unmanaged-short>	
        public int GrfStateBits;
        
        /// <summary>	
        /// No documentation.	
        /// </summary>	
        /// <include file='..\..\Documentation\CodeComments.xml' path="/comments/comment[@id='STATSTG::reserved']/*"/>	
        /// <unmanaged>unsigned int reserved</unmanaged>	
        /// <unmanaged-short>unsigned int reserved</unmanaged-short>	
        public int Reserved;

        // Internal native struct used for marshalling
        [StructLayout(LayoutKind.Sequential)]
        internal partial struct __Native {	
            public System.IntPtr PwcsName;
            public int Type;
            public long CbSize;
            public long Mtime;
            public long Ctime;
            public long Atime;
            public int GrfMode;
            public int GrfLocksSupported;
            public System.Guid Clsid;
            public int GrfStateBits;
            public int Reserved;
		    // Method to free unmanaged allocation
            internal unsafe void __MarshalFree()
            {   
                if (this.PwcsName != IntPtr.Zero)
                    Marshal.FreeHGlobal(this.PwcsName);		
            }
        }
		
		// Method to free unmanaged allocation
        internal unsafe void __MarshalFree(ref __Native @ref)
        {   
            @ref.__MarshalFree();
        }
		
		// Method to marshal from native to managed struct
        internal unsafe void __MarshalFrom(ref __Native @ref)
        {            
            this.PwcsName = ( @ref.PwcsName == IntPtr.Zero )?null:Marshal.PtrToStringUni(@ref.PwcsName);
            this.Type = @ref.Type;
            this.CbSize = @ref.CbSize;
            this.Mtime = @ref.Mtime;
            this.Ctime = @ref.Ctime;
            this.Atime = @ref.Atime;
            this.GrfMode = @ref.GrfMode;
            this.GrfLocksSupported = @ref.GrfLocksSupported;
            this.Clsid = @ref.Clsid;
            this.GrfStateBits = @ref.GrfStateBits;
            this.Reserved = @ref.Reserved;
        }
        // Method to marshal from managed struct tot native
        internal unsafe void __MarshalTo(ref __Native @ref)
        {
            @ref.PwcsName = ( this.PwcsName == null )?IntPtr.Zero : Utilities.StringToHGlobalUni(this.PwcsName);
            @ref.Type = this.Type;
            @ref.CbSize = this.CbSize;
            @ref.Mtime = this.Mtime;
            @ref.Ctime = this.Ctime;
            @ref.Atime = this.Atime;
            @ref.GrfMode = this.GrfMode;
            @ref.GrfLocksSupported = this.GrfLocksSupported;
            @ref.Clsid = this.Clsid;
            @ref.GrfStateBits = this.GrfStateBits;
            @ref.Reserved = this.Reserved;
		
		}
    }
}
#endif
