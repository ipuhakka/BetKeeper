PRAGMA foreign_keys=OFF;
BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS bets(
	bet_won INTEGER NOT NULL,
	name TEXT,
	odd REAL NOT NULL,
	bet REAL NOT NULL,
	date_time TEXT,
	owner INTEGER,	
	bet_id INTEGER PRIMARY KEY,
	FOREIGN KEY(owner) REFERENCES users(user_id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS users(
	username TEXT UNIQUE,
	password TEXT,
	user_id INTEGER PRIMARY KEY
);

CREATE TABLE IF NOT EXISTS bet_folders(
	folder_name TEXT,
	owner INTEGER,
	CONSTRAINT PK_BET_FOLDER PRIMARY KEY (folder_name, owner),
	FOREIGN KEY(owner) REFERENCES users(user_id) ON DELETE CASCADE 
);

CREATE TABLE IF NOT EXISTS bet_in_bet_folder(
	folder TEXT, 
	owner INTEGER, 
	bet_id INTEGER,
	CONSTRAINT PK_BET_IN_FOLDER PRIMARY KEY (folder, bet_id),
	FOREIGN KEY(bet_id) REFERENCES bets(bet_id) ON DELETE CASCADE 
	FOREIGN KEY(folder, owner) REFERENCES bet_folders(folder_name, owner) ON DELETE CASCADE 
);

COMMIT;
PRAGMA foreign_keys=ON;