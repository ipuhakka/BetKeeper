import React, { Component, Fragment } from 'react';
import _ from 'lodash';
import DropdownButton from 'react-bootstrap/DropdownButton';
import ListGroup from 'react-bootstrap/ListGroup';
import DropdownItem from 'react-bootstrap/DropdownItem';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import AddBet from '../../components/AddBet/AddBet.jsx';
import BetContainer from '../../containers/BetContainer.jsx';
import Filter from '../../components/Filter/Filter.jsx';
import Filters from '../../components/Filters/Filters.jsx';
import Header from '../../components/Header/Header.jsx';
import Info from '../../components/Info/Info.jsx';
import Menu from '../../components/Menu/Menu.jsx';
import UnresolvedBets from '../../components/UnresolvedBets/UnresolvedBets.jsx';
import ScrollableDiv from '../../components/ScrollableDiv/ScrollableDiv.jsx';
import './Bets.css';

class Bets extends Component
{
  constructor(props)
  {
		super(props);

		this.state = {
			menuDisabled: [false, true, false, false, false],
      showModal: false,
      visibleBets: null
    };
  }

  componentDidUpdate(prevProps)
  {
    const { props } = this;

    const previousBets = prevProps.betListFromFolder
      ? prevProps.betsFromFolder
      : prevProps.allBets;

    const currentBets = props.betListFromFolder
      ? props.betsFromFolder
      : props.allBets;

    if (!_.isEqual(currentBets, previousBets))
    {
      this.setState({
        visibleBets: currentBets
      })
    }
  }

  render()
  {
    const {props, state} = this;

    var betItems = this.renderBetsList();
    var menuItems = this.renderDropdown();
    var betView = this.renderBetView();

    const arrayToFilter = props.betListFromFolder ?
      props.betsFromFolder.bets : props.allBets;

    return (
      <div className="content">
    		<Header title={"Logged in as " + window.sessionStorage.getItem('loggedUser')}></Header>
    		<Menu disable={state.menuDisabled}></Menu>
    		<Info></Info>
        <i className="fas fa-plus-circle fa-2x addButton" onClick={this.showModal}></i>
        <AddBet show={state.showModal} hide={this.hideModal} folders={props.folders}/>
        <Row>
          <Col xs={12} md={{span: 6, order: 12}}>
            <div className="betView">
              {betView}
            </div>
          </Col>
          <Col xs={12} md={6}>
            <DropdownButton
              variant="primary"
              title={"Show from folder"}
              id={1}>
              {menuItems}
            </DropdownButton>
            <Filters
                toFilter={arrayToFilter}
                onResultsUpdate={this.onFilterUpdate}/>
            <ScrollableDiv className='betList'>                         
              <ListGroup>{betItems}</ListGroup>
            </ScrollableDiv>
          </Col>
        </Row>
      </div>);
  }

  // Displays either list of unresolved bets, or data of specific bet.
  renderBetView = () => 
  {
    const {props} = this;

    if (props.selectedBet !== -1)
    {
      let key = props.selectedBet;
      let bet = props.betListFromFolder ? props.betsFromFolder.bets[key] : props.allBets[key];

      return <BetContainer bet={bet} allFolders={props.folders} foldersOfBet={props.foldersOfBet}
       onDelete={props.onDeleteBet} updateFolders={props.getBetsFolders}></BetContainer>;
    }
    else {
      return props.unresolvedBets.length > 0 
        ? <UnresolvedBets bets={props.unresolvedBets}></UnresolvedBets> 
        : null;
    }
  }

  renderBetsList = () => 
  {
    const {props, state} = this;

    let bets = state.visibleBets;
    if (_.isNil(bets))
    {
      bets = props.betListFromFolder ? props.betsFromFolder.bets : props.allBets;
    }

		const betItems = [];
		var isSelected = false;

		for (var i = bets.length -1; i >= 0; i--){
			if (i === props.selectedBet || i.toString() === props.selectedBet)
				isSelected = true;
			else
				isSelected = false;

			var result = "Unresolved";

			if (bets[i].bet_won)
				result = "Won";
			else if (!bets[i].bet_won)
				result = "Lost";
			if (bets[i].bet_won === null || bets[i].bet_won.toString() === 'null')
				result = "Unresolved";

			betItems.push(<ListGroup.Item action
          onClick={props.onPressedBet.bind(this, i)} variant={isSelected ?  'info': null}
          key={i}>
          <div>{`${bets[i].name} ${bets[i].datetime} ${result}`}</div>
          <div className='small-betInfo'>{"Odd: " + bets[i].odd + " Bet: " + bets[i].bet}</div>
        </ListGroup.Item>)
		}

		return betItems;
	}

  renderFolderList = () => 
  {
    const {props} = this;
    var folderItems = [];

    for (var j = 0; j < props.foldersOfBet.length; j++){
      folderItems.push(
        <ListGroup.Item
          action
          onClick={this.onPressedFolder.bind(this, j)}
          variant={props.selectedFolders[j] ?  'info': null} key={j}>{props.folders[j]}
        </ListGroup.Item>)
    }

    return folderItems;
  }

  renderDropdown = () => 
  {
    const {props} = this;

    var menuItems = [];

    menuItems.push(<DropdownItem onClick={() => props.onShowFromFolder(-1)} key={-1}
      active={props.allFoldersSelected === -1} eventKey={-1}>{"show all"}</DropdownItem>);

    var active = false;

    for (var k = 0; k < props.folders.length; k++){
      active = false;
      if (k === props.allFoldersSelected || k.toString() === props.allFoldersSelected)
        active = true;
      menuItems.push(<DropdownItem onClick={props.onShowFromFolder.bind(this, k)} key={k} active={active}
        eventKey={k}>{props.folders[k]}</DropdownItem>);
    }

    return menuItems;
  }

  showModal = () => 
  {
    this.setState({
      showModal: true
    });
  }

  hideModal = () => 
  {
    this.setState({
      showModal: false
    });
  }

  renderFilters = () =>
  {
    const { props } = this;
    const arrayToFilter = props.betListFromFolder ?
      props.betsFromFolder.bets : props.allBets;

    return <Fragment>
      <Filter type="number"
        arrayToFilter={arrayToFilter}
        onUpdate={props.onFilterUpdate}
        filteredKey='bet'
        label='Bet' />

      <Filter type="number"
        arrayToFilter={arrayToFilter}
        onUpdate={props.onFilterUpdate}
        filteredKey='odd'
        label='Odd' />
    </Fragment>;
  }

  /*
  Handles an update in filtered list.
  */
  onFilterUpdate = (array) =>
  {
    this.setState({ visibleBets: array });
  }
};

export default Bets;
