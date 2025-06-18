using System.Collections.Generic;
using AllaganTetris.Extensions;

namespace AllaganTetris.Tetris
{
    public static class PieceFactory
    {
        #region Private Fields

        /// <summary>
        /// List of Tetris Pieces
        /// </summary>
        private static Dictionary<PieceType, Piece> _pieces = null!;
        private static Queue<PieceType> _piecesQueue = new();

        #endregion

        #region Constructors

        /// <summary>
        /// Adds some basic Tetris Pieces to the list
        /// </summary>
        static PieceFactory()
        {
            Initialize();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a specific Piece
        /// </summary>
        /// <param name="id">ID of Piece (0-6)</param>
        /// <returns>the Piece (or null if invalid Value)</returns>
        public static Piece? GetPiece(PieceType pieceType)
        {
            return _pieces.GetValueOrDefault(pieceType);
        }

        /// <summary>
        /// Returns a random Piece
        /// </summary>
        /// <returns>Random Piece</returns>
        public static Piece GetRandomPiece()
        {
            if (_piecesQueue.Count == 0)
            {
                FillBag();
            }
            return GetPiece(_piecesQueue.Dequeue())!;
        }

        public static PieceType PeekPiece()
        {
            if (_piecesQueue.Count == 0)
            {
                FillBag();
            }
            return _piecesQueue.Peek();
        }

        public static void FillBag()
        {
            List<PieceType> pieces =  [PieceType.I, PieceType.O, PieceType.L, PieceType.J, PieceType.S, PieceType.Z, PieceType.T];
            foreach (var piece in pieces.Shuffle())
            {
                _piecesQueue.Enqueue(piece);
            }
        }

        public static bool HasPiecesLeft()
        {
            return _pieces.Count != 0;
        }

        #endregion

        #region Public Properties
        public static int Count
        {
            get
            {
                return _pieces.Count;
            }
        }

        #endregion

        #region Helpers
        public static void Initialize()
        {
            _pieces = new Dictionary<PieceType, Piece>();

            //####
            _pieces.Add(PieceType.I, new Piece(PieceType.I, new int[,] { { 1, 1, 1, 1 } }));

            //##
            //##
            _pieces.Add(PieceType.O, new Piece(PieceType.O, new int[,] { { 2, 2 }, { 2, 2 } }));

            //  #
            //###
            _pieces.Add(PieceType.L, new Piece(PieceType.L, new int[,] { { 0, 0, 3 }, { 3, 3, 3 } }));

            //#
            //###
            _pieces.Add(PieceType.J, new Piece(PieceType.J, new int[,] { { 4, 0, 0 }, { 4, 4, 4 } }));

            // ##
            //##
            _pieces.Add(PieceType.S, new Piece(PieceType.S, new int[,] { { 0, 5, 5 }, { 5, 5, 0 } }));

            //##
            // ##
            _pieces.Add(PieceType.Z, new Piece(PieceType.Z, new int[,] { { 6, 6, 0 }, { 0, 6, 6 } }));

            //###
            // #
            _pieces.Add(PieceType.T, new Piece(PieceType.T, new int[,] { { 7, 7, 7 }, { 0, 7, 0 } }));
        }

        #endregion
    }
}

