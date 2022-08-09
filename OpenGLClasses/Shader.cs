using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;

namespace GameTest
{
    public class Shader
    {
        public readonly ShaderType shaderType;
        public readonly string shaderPath;
        public readonly string name;
        public readonly int shaderHandle;

        public readonly string infoLog;

        public Shader(ShaderType shaderType, string name, string shaderPath){
            this.shaderType = shaderType;
            this.name = name;
            this.shaderPath = shaderPath;
            this.shaderHandle = GL.CreateShader(shaderType);

            string code = File.ReadAllText(shaderPath);

            GL.ShaderSource(this.shaderHandle, code);
            GL.CompileShader(this.shaderHandle);
            
            this.infoLog = GL.GetShaderInfoLog(this.shaderHandle);

            if (infoLog != String.Empty){
                Console.WriteLine("{0} error : {1}", this.name, infoLog);
            }else{
                Console.WriteLine("{0} successfully compiled", this.name);
            }
        }

        public void DeleteShader(){
            GL.DeleteShader(this.shaderHandle);
        }
    }
}