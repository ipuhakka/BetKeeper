import React, { Component } from 'react';
import BetsContainer from '../../containers/BetsContainer.jsx';
import StatisticsContainer from '../../containers/StatisticsContainer.jsx';
import Folders from '../Folders/Folders.jsx';
import MenuCard from '../../components/MenuCard/MenuCard.jsx';
import Header from '../../components/Header/Header.jsx';
import Info from '../../components/Info/Info.jsx';
import Menu from '../../components/Menu/Menu.jsx';
import {changeToComponent} from '../../changeView';
import './Home.css';

class Home extends Component{

	constructor(props){
		super(props);

		this.state = {
			menuDisabled: [true, false, false, false, false],
			alertState: null,
			alertText: ""
		};
	}

	changePage = (key) => {
		switch (key){
			case 0:
				changeToComponent(<StatisticsContainer/>);
				break;
			case 1:
				changeToComponent(<BetsContainer/>);
				break;
			case 2:
				changeToComponent(<Folders/>);
				break;
			default:
				break;
		}
	}

	cardGrid = () => {
		return (
			<div className="grid">
				<MenuCard onClick={() => {this.changePage(0)}} image="fas fa-chart-bar fa-6x" title="Statistics" text="See how your bets have gone" data={this.props.stats}></MenuCard>
				<MenuCard onClick={() => {this.changePage(1)}} image="fas fa-pencil-alt fa-6x" title="Bets" text="Add and delete bets, update unresoved bets"></MenuCard>
				<MenuCard onClick={() => {this.changePage(2)}} image="fas fa-folder fa-6x" title="Folders" text="Create and delete folders"></MenuCard>
			</div>
		);
	}

	render() {
		return (
		<div className="content">
			<Header title={"Logged in as " + window.sessionStorage.getItem('loggedUser')}></Header>
			<Menu disable={this.state.menuDisabled}></Menu>
			<Info alertState={this.state.alertState} alertText={this.state.alertText} dismiss={this.dismissAlert}></Info>
			<div className="content">{this.cardGrid()}</div>
		</div>
		);
	}

	dismissAlert = () => {
		this.setState({
			alertState: null,
			alertText: ""
		});
	}

}

export default Home;
