/* This file contains functions for calculating statistics from arrays of bets.

Bet-object format:
        int owner;
        string name;
        string datetime;
        bool? bet_won; //null = unresolved, false = lost, true = won.
        int bet_id;
        double odd;
        double bet;

data parameter used in function is expected to be array parsed from JSON-data sent by the api.
*/
import {isOdd} from './utils.js';
import * as Sort from './sort.js';

//Counts how much money has been won in an array of bets. Money won in a single bet is (data["bet"] * data["odd"]).
export function moneyWon(data){
	var moneyWon = 0;
	for (var i = 0; i < data.length; i++){
		if (data[i]["bet_won"] === true){
			moneyWon = moneyWon + (data[i]["bet"] * data[i]["odd"]);
		}
	}
	return moneyWon;
}

/*Counts how much money has been played in the array of bets.*/
export function moneyPlayed(data){
	var moneyPlayed = 0;
	for (var i = 0; i < data.length; i++){
		if (data[i]["bet_won"] !== null && data[i]["bet_won"] !== 'null'){
			moneyPlayed = moneyPlayed + data[i]["bet"];
		}
	}
	return moneyPlayed;
}

export function moneyReturned(data){
	return moneyWon(data) - moneyPlayed(data);
}

export function playedBets(data){
	var played = 0;
	for (var i = 0; i < data.length; i++){
		if (data[i]["bet_won"] !== null && data[i]["bet_won"] !== 'null'){
			played = played + 1;
		}
	}
	return played;
}

export function wonBets(data){
	var won = 0;
	for (var i = 0; i < data.length; i++){
		if (data[i]["bet_won"] === true){
			won = won + 1;
		}
	}
	return won;
}

export function winPercentage(data){
	var percentage = wonBets(data) / playedBets(data);
	if (isNaN(percentage))
		return 0;
	return percentage;
}

//returns average return in an array of bets.
export function avgReturn(data){
	var avgRet = (moneyWon(data) - moneyPlayed(data)) / playedBets(data);

	if (isNaN(avgRet))
		return 0;
	return avgRet;
}

//Excpected return is median odd * win percentage.
export function expectedReturn(data){
	var played = playedBets(data);

	if (played === 0)
		return 0;
	return (median(data, "odd") * (wonBets(data) / played));
}

export function verifiedReturn(data){
	var verRet = moneyWon(data) / moneyPlayed(data);

	if (isNaN(verRet))
		return 0;
	return verRet;
}

///calculated median value for param in an array of bets.
export function median(data, param){
	var sorted = Sort.byHighest(data, param);
    if (sorted.length < 1)
    {
        return 0;
    }
    if (sorted.length  === 1)
    {
        return sorted[0][param];
    }


    if (isOdd(sorted.length))
    {
        return sorted[Math.floor(sorted.length / 2)][param];
    }
    else
    {
        return (sorted[Math.floor(sorted.length / 2)][param] + sorted[Math.floor(sorted.length / 2 - 1)][param]) / 2;
    }
}

//calculates mean value for specified parameter in an array of objects.
export function mean(data, param){
	if (data.length === 0)
		return 0;

	var sum = 0;
	for (var i = 0; i < data.length; i++){
		sum = sum + data[i][param];
	}
	return sum / data.length;
}

export function roundByTwo(num){
	return Math.round(num * 100) / 100;
}
