using System;
using OpenTK.Graphics.OpenGL4;

namespace GameTest
{
    public class ShaderProgram
    {
        public readonly int shaderProgramHandle;
        public readonly Shader[] shaders;

        public ShaderProgram(Shader[] shaders, bool detachAndDeleteShaders = true){
            this.shaderProgramHandle = GL.CreateProgram();
            this.shaders = shaders;

            foreach (Shader s in shaders){
                GL.AttachShader(this.shaderProgramHandle, s.shaderHandle);
            }

            GL.LinkProgram(this.shaderProgramHandle);

            if (detachAndDeleteShaders){
                this.DetachAndDeleteShaders();
            }
        }

        public void DetachAndDeleteShaders(){
            foreach (Shader s in shaders){
                GL.DetachShader(this.shaderProgramHandle, s.shaderHandle);
                s.DeleteShader();
            }
        }
    }
}