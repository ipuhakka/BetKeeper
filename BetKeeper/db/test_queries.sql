/* select bets from spesific owner and folder. */
select * from  bet_in_bet_folder bf
inner join bets b on b.bet_id = bf.bet_id
WHERE bf.owner = "jannu27" and bf.folder="valioliiga";

/* select all different folders from owner. */
select distinct folder from  bet_in_bet_folder bf
WHERE bf.owner = "jannu27";

select * from bets where owner="gaborik";

select * from bet_folders where owner="jannu27";

/* selects 1 if password is correct for user, 0 if not. */
SELECT (EXISTS (SELECT 1 FROM users WHERE username = "jannu27" and password = "wrong password"));