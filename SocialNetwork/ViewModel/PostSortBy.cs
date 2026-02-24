namespace SocialNetwork.ViewModel
{
    public enum PostSortBy
    {
        /// <summary>
        /// Bài vi?t m?i nh?t ??n c? nh?t (m?c ??nh)
        /// </summary>
        Newest = 0,

        /// <summary>
        /// Bài vi?t c? nh?t ??n m?i nh?t
        /// </summary>
        Oldest = 1,

        /// <summary>
        /// Nhi?u like nh?t ??n ít like
        /// </summary>
        MostLiked = 2,

        /// <summary>
        /// Nhi?u bình lu?n nh?t ??n ít bình lu?n
        /// </summary>
        MostCommented = 3,

        /// <summary>
        /// Ch? hi?n th? bài vi?t ?ã like
        /// </summary>
        Liked = 4
    }
}
