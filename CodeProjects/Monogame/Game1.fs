namespace Monogame

module Game1 = 
    open Microsoft.Xna.Framework
    open Microsoft.Xna.Framework.Graphics

    type Game1 () as this = 
        inherit Game ()

        abstract member graphics : GraphicsDeviceManager
        abstract member spriteBatch : SpriteBatch

        member this.graphics = new GraphicsDeviceManager (this)
        member this.spriteBatch = new SpriteBatch (this.graphics)
