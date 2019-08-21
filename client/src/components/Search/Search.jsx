import React, { Component } from 'react';
import PropTypes from 'prop-types';
import Form from 'react-bootstrap/Form';
import FormControl from 'react-bootstrap/FormControl';
import FormGroup from 'react-bootstrap/FormGroup';
import ListGroup from 'react-bootstrap/ListGroup';
import ListGroupItem from 'react-bootstrap/ListGroupItem';
import ScrollableDiv from '../ScrollableDiv/ScrollableDiv';
import './Search.css';

class Search extends Component {

  constructor(props){
    super(props);

    this.state = {
      shownData: [],
      searchValue: ""
    }
  }

  componentWillReceiveProps(nextProps){
    const {data} = nextProps;

    this.setState({
      shownData: data
    })
  }

  render(){
    return (
      <div>
        <Form>
          <FormGroup>
            <FormControl
              placeholder={this.props.placeholder}
              type="text"
              value={this.state.searchValue}
              onChange={this.updateSearch}/>
            <ScrollableDiv className="overflow-list">
              <ListGroup>{this.renderShowedResults()}</ListGroup>
            </ScrollableDiv>
            
          </FormGroup>
        </Form>
      </div>
    );
  }

  /* Renders the list of showed results.*/
  renderShowedResults(){
    const {state, props} = this;
    const key = this.props.key;

    let listGroupItems = [];

    for (var i = 0; i < state.shownData.length; i++){
      if (i > props.showCount){
        break;
      }

      listGroupItems.push(
        <ListGroupItem action onClick={this.pressedItem.bind(this, state.shownData[i])} key={i}>
          {key === null ? state.shownData[i] : state.shownData[i].key}</ListGroupItem>);
    }

    return listGroupItems;
  }

  pressedItem = (data, e) => {
    const {props} = this;
    e.preventDefault();

    if (props.clearOnClick){
      this.setState({
        searchValue: "",
        shownData: []
      });
    }

    props.onClickResult(data);
  }

  /*Updates search result. Sets the shownResults
  as all results which include searched string. */
  updateSearch = (e) => {
    const {props} = this;

    let inputText = e.target.value;
    let searchedSubstring = this.formSearchWord(inputText);

    this.setState({
      searchValue: inputText
    }, () => {

      let shownResultsArray = [];
      let searchedKey = props.key;

      for (var i = 0; i < props.data.length; i++){
        let compareWord = (searchedKey === null ?
          props.data[i] : props.data[i].searchedKey).toLowerCase();

        if (compareWord.indexOf(searchedSubstring) !== -1){
          shownResultsArray.push(props.data[i]);
        }
      }

      this.setState({
        shownData: shownResultsArray
      });
    });
  }

  /* Returns a filtered search word.*/
  formSearchWord = (originalWord) => {
    if (typeof(originalWord) === 'string'){
      return originalWord.toLowerCase();
    }

    return originalWord.toString();
  }
}

Search.defaultProps = {
  key: null,
  clearOnClick: false
};

Search.propTypes = {
  data: PropTypes.array.isRequired,
  onClickResult: PropTypes.func.isRequired,
  key: PropTypes.string,
  placeholder: PropTypes.string,
  clearOnClick: PropTypes.bool,
};

export default Search;
