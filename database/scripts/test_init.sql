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

INSERT OR REPLACE INTO users (username, password) VALUES("jannu27", "salasana");
INSERT OR REPLACE INTO users (username, password) VALUES("käyttäjä", "salasana2");
INSERT OR REPLACE INTO users (username, password) VALUES("käyttäjä2", "salasana3");
INSERT OR REPLACE INTO users (username, password) VALUES("gaborik", "salainensalasana");
INSERT OR REPLACE INTO users (username, password) VALUES("rooni", "onihanhyvä123");

INSERT OR REPLACE INTO bet_folders VALUES("valioliiga", 1);
INSERT OR REPLACE INTO bet_folders VALUES("liiga", 1);
INSERT OR REPLACE INTO bet_folders VALUES("huumoribetsit", 2);
INSERT OR REPLACE INTO bet_folders VALUES("tuplat", 3);
INSERT OR REPLACE INTO bet_folders VALUES("rooneynBetsit", 5);
INSERT OR REPLACE INTO bet_folders VALUES("triplat", 1);

INSERT OR REPLACE INTO bets (name, odd, bet, date_time, owner, bet_won, bet_id) VALUES (NULL, 2.64, 3, datetime('now', 'localTime'), 1, 0, 1);
INSERT OR REPLACE INTO bets (name, odd, bet, date_time, owner, bet_won, bet_id) VALUES(NULL, 3.13, 3, datetime('now', 'localTime'), 1, 0, 2);
INSERT OR REPLACE INTO bets (name, odd, bet, date_time, owner, bet_won, bet_id) VALUES (NULL, 13, 3.3, datetime('now', 'localTime'), 1, 0, 3);
INSERT OR REPLACE INTO bets (name, odd, bet, date_time, owner, bet_won, bet_id) VALUES (NULL, 4, 3.3, datetime('now', 'localTime'), 3, 0, 4);
INSERT OR REPLACE INTO bets (name, odd, bet, date_time, owner, bet_won, bet_id) VALUES ("Varma betsi", 100, 2.0, datetime('now', 'localTime'), 1, -1, 5);
INSERT OR REPLACE INTO bets (name, odd, bet, date_time, owner, bet_won, bet_id) VALUES ("Varma betsi", 200, 1.56, datetime('now', 'localTime'),4, -1, 6);
INSERT OR REPLACE INTO bets (name, odd, bet, date_time, owner, bet_won, bet_id) VALUES ("Varma betsi", 200, 1.56, datetime('now', 'localTime'), 5, -1, 7);
INSERT OR REPLACE INTO bets (name, odd, bet, date_time, owner, bet_won, bet_id) VALUES (NULL, 5, 3.15, datetime('now', 'localTime'), 4, 1, 8);
INSERT OR REPLACE INTO bets (name, odd, bet, date_time, owner, bet_won, bet_id) VALUES (NULL, 6, 2, datetime('now', 'localTime'), 5, 1, 9);

INSERT OR REPLACE INTO bet_in_bet_folder VALUES("valioliiga", 1, 1);
INSERT OR REPLACE INTO bet_in_bet_folder VALUES("valioliiga", 1, 2);
INSERT OR REPLACE INTO bet_in_bet_folder VALUES("liiga", 1, 3);
INSERT OR REPLACE INTO bet_in_bet_folder VALUES("tuplat", 3, 4);
INSERT OR REPLACE INTO bet_in_bet_folder VALUES("valioliiga", 1, 5);
INSERT OR REPLACE INTO bet_in_bet_folder VALUES("rooneynBetsit", 5, 7);
INSERT OR REPLACE INTO bet_in_bet_folder VALUES("rooneynBetsit", 5, 9);

COMMIT;
PRAGMA foreign_keys=ON;
