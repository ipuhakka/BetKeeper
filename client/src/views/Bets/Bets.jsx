import React, { Component, Fragment } from 'react';
import _ from 'lodash';
import ListGroup from 'react-bootstrap/ListGroup';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import AddBet from '../../components/AddBet/AddBet';
import BetContainer from '../../containers/BetContainer';
import Dropdown from '../../components/Dropdown/Dropdown';
import Filter from '../../components/Filter/Filter';
import Filters from '../../components/Filters/Filters';
import Header from '../../components/Header/Header';
import Info from '../../components/Info/Info';
import Menu from '../../components/Menu/Menu';
import UnresolvedBets from '../../components/UnresolvedBets/UnresolvedBets';
import ScrollableDiv from '../../components/ScrollableDiv/ScrollableDiv';
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

    this.betViewRef = React.createRef();
  }

  render()
  {
    const {props, state} = this;

    const betItems = this.renderBetsList();
    const betView = this.renderBetView();

    const arrayToFilter = props.betListFromFolder 
      ? props.betsFromFolder.bets 
      : props.allBets;

    const folderSelections = _.clone(props.folders);

    folderSelections.splice(0, 0, 'Overview');

    return (
      <div className="content">
    		<Header title={"Logged in as " + window.sessionStorage.getItem('loggedUser')}></Header>
    		<Menu disable={state.menuDisabled}></Menu>
    		<Info></Info>
        <i className="fas fa-plus-circle fa-2x addButton" onClick={this.showModal}></i>
        <AddBet show={state.showModal} hide={this.hideModal} folders={props.folders}/>
        <Row>
          <Col xs={12} md={{span: 6, order: 12}}>
            <div className="betView" ref={this.betViewRef}>
              {betView}
            </div>
          </Col>
          <Col xs={12} md={6}>
            <Dropdown 
              data={folderSelections}
              title='Select from folder'
              selectedItemAsTitle
              stateKey=''
              id='bets_folder_dropdown'
              onUpdate={(key, stateKey) => 
              {
                props.onShowFromFolder(key - 1);
              }}
              />
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

  /**
   * Displays either list of unresolved bets, or data of specific bet.
   */
  renderBetView = () => 
  {
    const {props} = this;

    if (props.selectedBet !== -1)
    {
      let key = props.selectedBet;
      let bet = props.betListFromFolder ? props.betsFromFolder.bets[key] : props.allBets[key];

      return <BetContainer 
        onClose={() => this.handleBetListClick(props.selectedBet)} 
        bet={bet} 
        allFolders={props.folders} 
        foldersOfBet={props.foldersOfBet}
        onDelete={props.onDeleteBet} 
        updateFolders={props.getBetsFolders}></BetContainer>;
    }
    else 
    {
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

    for (var i = bets.length -1; i >= 0; i--)
    {
			if (i === props.selectedBet || i.toString() === props.selectedBet)
				isSelected = true;
			else
				isSelected = false;

      var result = "Unresolved";
      
			if (bets[i].betResult === 1)
				result = "Won";
			else if (bets[i].betResult === 0)
				result = "Lost";
			if (bets[i].betResult === -1)
				result = "Unresolved";

      betItems.push(<ListGroup.Item 
        action
        onClick={this.handleBetListClick.bind(this, i)} 
        variant={isSelected ?  'info': null}
        key={i}>     
        <div>{`${bets[i].name} ${bets[i].playedDate} ${result}`}</div>
        <div className='small-betInfo'>{`Odd: ${bets[i].odd} Bet:  ${bets[i].stake}`}</div>
        </ListGroup.Item>);
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

  handleBetListClick = (i) => 
  {
    this.props.onPressedBet(i);

    window.scrollTo({
      left: 0, 
      top: this.betViewRef.current.offsetTop, 
      behavior: 'smooth'
    });
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
