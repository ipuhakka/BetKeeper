import React, { Component } from 'react';
import PropTypes from 'prop-types';
import Form from 'react-bootstrap/lib/Form';
import FormControl from 'react-bootstrap/lib/FormControl';
import FormGroup from 'react-bootstrap/lib/FormGroup';
import ListGroup from 'react-bootstrap/lib/ListGroup';
import ListGroupItem from 'react-bootstrap/lib/ListGroupItem';

class Search extends Component {

  constructor(props){
    super(props);

    this.state = {
      shownData: [],
      minimumCharacters: 3,
      searchValue: ""
    }
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
            <ListGroup>{this.renderShowedResults()}</ListGroup>
          </FormGroup>
        </Form>
      </div>
    );
  }

  /* Renders the list of showed results.*/
  renderShowedResults(){
    let key = this.props.key;

    if (this.canShowAll()){
      let i = -1;

      return this.props.data.map(row => {
        i = i + 1;
          return <ListGroupItem onClick={this.props.onClickResult.bind(this, this.props.data[i])} key={i}>
          {key === null ? this.props.data[i] : this.props.data[i].key}</ListGroupItem>;
      });
    }
    else {
      let listGroupItems = [];

      for (var i = 0; i < this.state.shownData.length; i++){
        if (i > this.props.showCount){
          break;
        }

        listGroupItems.push(
          <ListGroupItem onClick={this.props.onClickResult.bind(this, this.state.shownData[i])} key={i}>
            {key === null ? this.state.shownData[i] : this.state.shownData[i].key}</ListGroupItem>);
      }

      return listGroupItems;
    }
  }

  /*Updates search result. Sets the shownResults
  as all results which include searched string. */
  updateSearch = (e) => {
    let inputText = e.target.value;
    let searchedSubstring = this.formSearchWord(inputText);

    this.setState({
      searchValue: inputText
    }, () => {

      if (searchedSubstring.length < this.state.minimumCharacters){
        this.setState({
          shownData: []
        });
      }
      else {
        let shownResultsArray = [];
        let searchedKey = this.props.key;

        for (var i = 0; i < this.props.data.length; i++){
          let compareWord = (searchedKey === null ?
            this.props.data[i] : this.props.data[i].searchedKey).toLowerCase();

          if (compareWord.indexOf(searchedSubstring) !== -1){
            shownResultsArray.push(this.props.data[i]);
          }
        }

        this.setState({
          shownData: shownResultsArray
        });
      }
    });
  }

  /* Returns a filtered search word.*/
  formSearchWord = (originalWord) => {
    if (typeof(originalWord) === 'string'){
      return originalWord.toLowerCase();
    }

    return originalWord.toString();
  }

  /* Returns true if all data in array can be shown. */
  canShowAll = () => {
    return this.props.showCount >= this.props.data.length;
  }
}

Search.defaultProps = {
  showCount: 5,
  key: null
};

Search.propTypes = {
  data: PropTypes.array.isRequired,
  onClickResult: PropTypes.func.isRequired,
  key: PropTypes.string,
  showCount: PropTypes.number,
  placeholder: PropTypes.string
};

export default Search;
