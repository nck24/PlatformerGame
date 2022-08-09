#version 330 core
                
in vec4 vColor;  //Mora se ime ujemati z imenom prejsnega shaderja

out vec4 pixelColor;
void main(){
    pixelColor = vColor;
}