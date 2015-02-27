namespace Monogame

module Game1 = 
    open Microsoft.Xna.Framework
    open Microsoft.Xna.Framework.Content
    open Microsoft.Xna.Framework.Graphics

    type Game1 () as this = 
        inherit Game ()


        do 
            let contentmanager = new Content.ContentManager ()
            // this.alien <- 

        abstract member alien : Texture2D

        // abstract member graphics : GraphicsDeviceManager
        // abstract member spriteBatch : SpriteBatch
        // 
        // member this.graphics = new GraphicsDeviceManager (this)
        // member this.spriteBatch = new SpriteBatch (this.graphics)
