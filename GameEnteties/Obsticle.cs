using OpenTK.Mathematics;

namespace GameTest
{
    public class Obsticle
    {
        public Rectangle obsticleRect;
        private Vector2 size;
        private Vector4 color;
        public Vector2 speed;
        public const int verteciesLenght = Rectangle.verteciesLenght;
        public const int indiciesLenght = Rectangle.indiciesLenght;

        public Obsticle(Rectangle obsticleRect){
            this.obsticleRect = obsticleRect;
            this.size = this.obsticleRect.GetSize();
            this.color = this.obsticleRect.GetColor();
            this.speed = Vector2.Zero;
        }

        public Obsticle(Rectangle obsticleRect, Vector2 speed){
            this.obsticleRect = obsticleRect;
            this.size = this.obsticleRect.GetSize();
            this.color = this.obsticleRect.GetColor();
            this.speed = speed;
        }

        public void ChangeRectangle(Rectangle newRectangle){
            this.obsticleRect = newRectangle;
            this.size = this.obsticleRect.GetSize();
            this.color = this.obsticleRect.GetColor();
        }

        public void ChangeColor(Vector4 newColor){
            Rectangle newRect = new Rectangle(obsticleRect.GetStartPoint(), this.size, newColor);
            this.color = newColor;
            this.obsticleRect = newRect;
        }

        public void Move(float deltaT){
            Vector2 change = speed * deltaT;
            obsticleRect.Move(change);
        }

        public void Teleport(Vector2 newStartPoint){
            this.obsticleRect = new Rectangle(newStartPoint, this.size, this.color);
        }

        public void ChangeSize(Vector2 newSize){
            Vector2 startPoint = this.obsticleRect.GetStartPoint();
            this.size = newSize;
            Rectangle newRectangle = new Rectangle(startPoint, this.size, this.color);
            this.ChangeRectangle(newRectangle);
        }

        public bool IsColliding(Rectangle entety){
            return ( IsInRange(entety.GetUpY(), this.obsticleRect.GetUpY(), this.obsticleRect.GetDownY()) ||
                IsInRange(entety.GetDownY(), this.obsticleRect.GetUpY(), this.obsticleRect.GetDownY()) ) &&
                ( IsInRange(entety.GetLeftX(), this.obsticleRect.GetLeftX(), this.obsticleRect.GetRightX()) ||
                IsInRange(entety.GetRightX(), this.obsticleRect.GetLeftX(), this.obsticleRect.GetRightX()) )
                ||
                (IsInRange(this.obsticleRect.GetUpY(), entety.GetUpY(), entety.GetDownY()) ||
                IsInRange(this.obsticleRect.GetDownY(), entety.GetUpY(), entety.GetDownY()) ) &&
                ( IsInRange(this.obsticleRect.GetLeftX(), entety.GetLeftX(), entety.GetRightX()) ||
                IsInRange(this.obsticleRect.GetRightX(), entety.GetLeftX(), entety.GetRightX()) );
        }

        private bool IsInRange(float val, float end1, float end2){
            return val >= MathF.Min(end1, end2) && val <= MathF.Max(end1, end2);
        }

        private T NextElementInLoopedArray<T>(T[] array, int currentElem){
            if (currentElem >= array.Length){
                throw new ArgumentOutOfRangeException(nameof(currentElem));
            }
            if (currentElem < array.Length - 2){
                return array[currentElem + 1];
            }else{
                return array[0];
            }
        }
        
        public CollisionResolvment? GetRectVsRectCollisionResolvment(Rectangle entety){
            Vector4[] thisRectSegments = GetSegments(this.obsticleRect);
            Vector4[] entetyRectSegments = GetSegments(entety);
            Vector2 entetyCentre = new Vector2((entety.GetLeftX() + entety.GetRightX()) / 2, (entety.GetUpY() + entety.GetDownY()) / 2);
            Vector2 thisRectCentre = new Vector2((this.obsticleRect.GetLeftX() + this.obsticleRect.GetRightX()) / 2,
                (this.obsticleRect.GetUpY() + this.obsticleRect.GetDownY()) / 2);
            //For special cases
            float xWhole = this.size.X / 2 + entety.GetSize().X / 2;
            float yWhole = this.size.Y / 2 + entety.GetSize().Y / 2;
            Vector2 centVector = new Vector2(entetyCentre.X - thisRectCentre.X, entetyCentre.Y - thisRectCentre.Y);
            float xChange = Math.Abs(centVector.X / xWhole);
            float yChange = Math.Abs(centVector.Y / yWhole);
            // Top / right case
            if (entetyCentre.X > thisRectCentre.X && entetyCentre.Y < thisRectCentre.Y){
                if (AreSegmentsIntersecting(thisRectSegments[0], entetyRectSegments[3])&&AreSegmentsIntersecting(thisRectSegments[1], entetyRectSegments[2])){
                    float xLenght = thisRectSegments[0].Z - entetyRectSegments[0].X;
                    float yLenght = entetyRectSegments[2].Y - thisRectSegments[0].Y;
                    if (xLenght < yLenght){
                        return CollisionResolvment.Right;
                    }
                    else{
                        return CollisionResolvment.Up;
                    }
                }

                // Intersecting only up
                if (yChange > xChange){
                    return CollisionResolvment.Up;
                }
                // Intersectnig only right
                if (xChange > yChange){
                    return CollisionResolvment.Right;
                }
            }
            // Down / right case
            if (entetyCentre.X > thisRectCentre.X && entetyCentre.Y > thisRectCentre.Y){
                if (AreSegmentsIntersecting(thisRectSegments[1], entetyRectSegments[0])&&AreSegmentsIntersecting(thisRectSegments[2], entetyRectSegments[3])){
                    float xLenght = thisRectSegments[1].X - entetyRectSegments[0].X;
                    float yLenght = thisRectSegments[2].Y - entetyRectSegments[0].Y;
                    if (xLenght < yLenght){
                        return CollisionResolvment.Right;
                    }
                    else{
                        return CollisionResolvment.Down;
                    }
                }
                
                // Intersecting only down
                if (yChange > xChange){
                    return CollisionResolvment.Down;
                }
                // Intersectnig only right
                if (xChange > yChange){
                    return CollisionResolvment.Right;
                }
            }
            // Down / left case
            if (entetyCentre.X < thisRectCentre.X && entetyCentre.Y > thisRectCentre.Y){
                if (AreSegmentsIntersecting(thisRectSegments[2], entetyRectSegments[1])&&AreSegmentsIntersecting(thisRectSegments[3], entetyRectSegments[0])){
                    float xLenght = entetyRectSegments[0].Z - thisRectSegments[3].X;
                    float yLenght = thisRectSegments[2].Y - entetyRectSegments[0].Y;
                    if (xLenght < yLenght){
                        return CollisionResolvment.Left;
                    }
                    else{
                        return CollisionResolvment.Down;
                    }
                }
                // Intersecting only down
                if (yChange > xChange){
                    return CollisionResolvment.Down;
                }
                // Intersectnig only left
                if (xChange > yChange){
                    return CollisionResolvment.Left;
                }
            }
            // Up / left case
            if (entetyCentre.X < thisRectCentre.X && entetyCentre.Y < thisRectCentre.Y){
                if (AreSegmentsIntersecting(thisRectSegments[0], entetyRectSegments[1])&&AreSegmentsIntersecting(thisRectSegments[3], entetyRectSegments[2])){
                    float xLenght = entetyRectSegments[0].Z - thisRectSegments[3].X;
                    float yLenght = entetyRectSegments[2].Y - thisRectSegments[0].Y;
                    if (xLenght < yLenght){
                        return CollisionResolvment.Left;
                    }
                    else{
                        return CollisionResolvment.Up;
                    }
                }
                // Intersecting only up
                if (yChange > xChange){
                    return CollisionResolvment.Up;
                }
                // Intersectnig only left
                if (xChange > yChange){
                    return CollisionResolvment.Left;
                }
            }
            if (entetyCentre.X == thisRectCentre.X && entetyCentre.Y <= thisRectCentre.Y){
                return CollisionResolvment.Up;
            }
            if (entetyCentre.X == thisRectCentre.X && entetyCentre.Y >= thisRectCentre.Y){
                return CollisionResolvment.Down;
            }
            if (entetyCentre.Y == thisRectCentre.Y && entetyCentre.X <= thisRectCentre.X){
                return CollisionResolvment.Left;
            }
            if (entetyCentre.Y == thisRectCentre.Y && entetyCentre.X >= thisRectCentre.X){
                return CollisionResolvment.Right;
            }
            return null;
        }

        private bool AreSegmentsIntersecting(Vector4 seg1, Vector4 seg2){
            float tTop = (seg2.Z - seg2.X) * (seg1.Y - seg2.Y) - (seg2.W - seg2.Y) * (seg1.X - seg2.X);
            float uTop = (seg2.Y - seg1.Y) * (seg1.X - seg1.Z) - (seg2.X - seg1.X) * (seg1.Y - seg1.W);
            float bottom = (seg2.W -seg2.Y) * (seg1.Z - seg1.X) - (seg2.Z - seg2.X) * (seg1.W - seg1.Y);
            if (bottom != 0){
                float t = tTop / bottom;
                float u = uTop / bottom;
                if (IsInRange((float)t, 0f, 1f) && IsInRange((float)u , 0f, 1f)){
                    return true;
                }
            }
            return false;
        }

        // Returns a segment in format : x1,y1,x2,y2
        private Vector4[] GetSegments(Rectangle rect){
            Vector4[] segments = new Vector4[4];
            VertexPositionColor[] vertices = rect.GetVerticies();

            segments[0] = new Vector4(vertices[0].position.X, vertices[0].position.Y, vertices[1].position.X, vertices[1].position.Y);
            segments[1] = new Vector4(vertices[1].position.X, vertices[1].position.Y, vertices[2].position.X, vertices[2].position.Y);
            segments[2] = new Vector4(vertices[3].position.X, vertices[3].position.Y, vertices[2].position.X, vertices[2].position.Y);
            segments[3] = new Vector4(vertices[0].position.X, vertices[0].position.Y, vertices[3].position.X, vertices[3].position.Y);

            return segments;
        }
    }

    public enum CollisionResolvment{
        Up,
        Left,
        Down,
        Right
    }
}