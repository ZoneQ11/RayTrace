Glenn Kenneth Wijono, 6564968
Mirco Clement, 6129196
--- --- ---
Distribution of Work:
We did not make any promises in the long term, so it was more of a each for himself situation where we use the better version compared to each other.
Mirco already had deadlines for other assignments a couple of days before this project's deadline, which caused Glenn to do the majority of the basic work such as making the Ray Class.

Ray Class - Glenn
Light Class - Glenn
Coloring Light - Glenn
Primitive Class - Glenn
Circle Class - Glenn
Light in Circle Class - Glenn
Circle Intersect() - Mirco
Line Class - Glenn
Line Intersect() - Glenn

Body within Init() - Glenn
Implementation of code within RenderGL() - Glenn
--- --- ---
How to operate:
Despite the fact that the program "should" be trivial, an explanation will be given by Glenn regardless, for safety measures.

The Classes that are found in Primitives.cs are the Classes managing the objects.
They are the bones of the program, alongside with the RenderGL() within the MyApplication Class, and should not be changed in any way.
(Unless it is for improvements, but we do not have the time such luxury.)

If you wish to add any primitives or lights, then you would have to make a new one within the MyApplication Class.
Personally, I think that it could have been more efficient one way or the other, but as I don't know how to improve it, I can only leave them as they are.
Each circle have a light inside them, hence why they need four arguments each time they are made.
The f float is a function to transform the boundaries. E.g. from [-1, 1] to [0, 255].

Within Init(), the values of the primitives and lights are to be written down, and the primitives and lights themselves are to be added within their own Lists.
Given that each circle has a light within them, it might be inefficient in the long term, and adding them is thereby optional.

Within Tick(), the position of the light sources (or any values, really) are constantly updated.
The current effect is to make the light rotate clockwise and nothing else.

Within RenderGL, the actual body of the whole program is being run. The code was made thanks to the pseudo-code given along with the assignment.
We attempted to implement multi-threading via Parallel.For() method. However, it unfortunately did not work well for us as neither of us knew how to implement the method.
--- --- ---
Sources:
We used the algebra formulas from the slides from the lectures and from the websites https://ncase.me/sight-and-light/, Wikipedia, and other forums such as stackoverflow.com.