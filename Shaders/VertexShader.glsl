#version 330 core
uniform vec2 ViewportSize;
//uniform float colorFactor;

layout (location = 0) in vec2 aPosition;
layout (location = 1) in vec4 aColor;
out vec4 vColor; //Ta gre vn k naslednjemu shaderju (fragment shader v tem primeru)

void main(){
    float nx = aPosition.x / ViewportSize.x * 2.0f - 1.0f;
    //float ny = (aPosition.y / ViewportSize.y * 2.0f - 1.0f) * -1.0f;
    float ny = 1.0f - 2.0f * aPosition.y / ViewportSize.y;

    vColor = aColor; //* colorFactor;
    gl_Position = vec4(nx, ny, 0.0f, 1.0f);
}