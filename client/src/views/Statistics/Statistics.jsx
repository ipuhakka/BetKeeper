import React, { Component } from 'react';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import BarGraph from '../../components/BarGraph/BarGraph.jsx';
import Dropdown from '../../components/Dropdown/Dropdown.jsx';
import Header from '../../components/Header/Header.jsx';
import Info from '../../components/Info/Info.jsx';
import Menu from '../../components/Menu/Menu.jsx';
import ObjectTable from '../../components/ObjectTable/ObjectTable.jsx';
import OverviewTable from '../../components/OverviewTable/OverviewTable.jsx';
import ScatterPlot from '../../components/ScatterPlot/ScatterPlot.jsx';
import './Statistics.css';

class Statistics extends Component{
	constructor(props){
		super(props);

		this.state = {
			disabled: [false, false, true, false, false],
			folderSelected: 0
		};
	}

	render(){
		const {state, props} = this;

		return(
			<div className="content">
				<Header title={"Logged in as " + window.sessionStorage.getItem('loggedUser')}></Header>
				<Menu disable={state.disabled}></Menu>
				<Info></Info>
				<div>
					<Row>
						<Col md={6} xs={12}>
							<BarGraph data={props.betStatistics} optionLabels={this.graphOptionLabels()}
								graphOptions={props.graphOptions} />
							<div>
								<OverviewTable defaultSort={props.betStatistics} overviewItems={props.betStatistics}/>
							</div>
						</Col>
						<Col md={6} xs={12}>
							<Dropdown
								variant="primary"
								title={"Show folder"}
								id={2}
								data={this.folders()}
								onUpdate={this.setSelectedDropdownItem.bind(this)}
								stateKey={"folderSelected"}>
							</Dropdown>
							{this.renderTable()}
						</Col>
					</Row>
					<Row>
						<Col>
								<ScatterPlot optionLabels={this.graphOptionLabels()} data={props.betStatistics}
								graphOptions={props.graphOptions}></ScatterPlot>
						</Col>
					</Row>
				</div>
			</div>
		);
	}

	renderTable = () => {
		const {folderSelected} = this.state;
		const { graphOptions, betStatistics} = this.props;

		const titleOptions = graphOptions.map(option => {
			return ({
				key: option.variableName,
				value: option.labelName
			});
		})

		return betStatistics.length > 0 ?
				<ObjectTable
						tableTitle={betStatistics[folderSelected].folder}
						data={betStatistics[folderSelected]}
						titles={titleOptions}/> : null;
	}

	folders = () => {
		return this.props.betStatistics.map(folder => {
			return folder.folder;
		})
	};

	graphOptionLabels = () => {
		return this.props.graphOptions.map(option => {
			return option.labelName;
		});
	}

	setSelectedDropdownItem(key, stateKey){
		this.setState({
			[stateKey]: key
		});
	}
}

export default Statistics;
