using System;
using Server.Lib.Infrastructure;

namespace Server.Lib.Models.Resources.Posts
{
    public class PostType
    {
        #region Constructors.

        public static PostType FromString(string postType, bool forceWildcard = false)
        {
            Ensure.Argument.IsNotNullOrWhiteSpace(postType, nameof(postType));

            // Normalize the provided type.
            postType = postType.Trim().ToLower();
            return PostType.FromUri(new Uri(postType, UriKind.Absolute), forceWildcard);
        }

        public static PostType FromUri(Uri postTypeUri, bool forceWildcard = false)
        {
            Ensure.Argument.IsNotNull(postTypeUri, nameof(postTypeUri));

            // Create a new Post Type from the provided Uri.
            return new PostType(
                postTypeUri.AbsoluteUri.Substring(0, postTypeUri.AbsoluteUri.Length - postTypeUri.Fragment.Length),
                forceWildcard ? null : string.IsNullOrEmpty(postTypeUri.Fragment) || postTypeUri.Fragment.Length <= 1 ? string.Empty : postTypeUri.Fragment.Substring(1),
                forceWildcard || string.IsNullOrEmpty(postTypeUri.Fragment)
            );
        }

        private PostType(
            string type,
            string variant,
            bool isWildcard = false)
        {
            this.Type = type;
            this.Variant = variant;
            this.IsWildcard = isWildcard;
        }

        #endregion

        #region Public properties.

        public string Type { get; }
        public string Variant { get; }
        public bool IsWildcard { get; }

        #endregion

        #region Public interface.

        /// <summary>
        /// Create a new <see cref="PostType"/> with a different Variant.
        /// </summary>
        /// <param name="variant">The Variant of the new <see cref="PostType"/>.</param>
        /// <returns>A new <see cref="PostType"/> with the specified Variant.</returns>
        public PostType ToVariant(string variant)
        {
            Ensure.Argument.IsNotNullOrWhiteSpace(variant, nameof(variant));
            return new PostType(this.Type, variant);
        }

        public override string ToString()
        {
            return this.IsWildcard
                ? this.Type
                : $"{this.Type}#{this.Variant}";
        }

        public bool Equals(PostType other)
        {
            return this.ToString() == other.ToString();
        }

        public override bool Equals(object other)
        {
            return this.Equals(other as PostType);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        #endregion
    }
}