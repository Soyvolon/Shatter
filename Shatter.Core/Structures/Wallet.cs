using System;
using System.ComponentModel.DataAnnotations;

namespace Shatter.Core.Structures
{
	public class Wallet
    {
        [Key]
        public ulong UserId { get; private set; }

        public int Balance { get; private set; }
        public string Username { get; set; }
        public DateTime LastDaily { get; set; }


        public Wallet(ulong userId) : this(userId, "")
        {

        }

        public Wallet(ulong userId, string user)
        {
			this.UserId = userId;
			this.Balance = 100;
			this.Username = user;
			this.LastDaily = DateTime.MinValue;
        }

        /// <summary>
        /// Adds to the users balance.
        /// </summary>
        /// <param name="amount">Ammount to add.</param>
        /// <returns>The new balance.</returns>
        public int Add(int amount)
        {
			this.Balance += amount;
            return this.Balance;
        }

        /// <summary>
        /// Subtracts to the users balance.
        /// </summary>
        /// <param name="amount">Ammount to subtract.</param>
        /// <returns>The new balance.</returns>
        public int Subtract(int amount)
        {
			this.Balance -= amount;
            return this.Balance;
        }

        /// <summary>
        /// Checks to see if the user has enough money.
        /// </summary>
        /// <param name="amount">Value to check against.</param>
        /// <returns>True if the user has more money than the value passed to <paramref name="amount"/></returns>
        public bool HasEnough(int amount = 0) => this.Balance >= amount;

        /// <summary>
        /// Transfers an ammount to another wallet.
        /// </summary>
        /// <param name="ammount">Ammount to transfer</param>
        /// <param name="to">Wallet to transfer to.</param>
        /// <returns>Money reamining.</returns>
        public int Transfer(int ammount, Wallet to)
        {
            to.Add(ammount);
            return Subtract(ammount);
        }
    }
}
