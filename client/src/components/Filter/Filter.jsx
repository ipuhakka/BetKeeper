import React, { Component } from 'react';
import PropTypes from 'prop-types';
import ControlLabel from 'react-bootstrap/lib/ControlLabel';
import Form from 'react-bootstrap/lib/Form';
import FormControl from 'react-bootstrap/lib/FormControl';
import FormGroup from 'react-bootstrap/lib/FormGroup';
import Dropdown from '../Dropdown/Dropdown.jsx';
import './Filter.css';

const selections =
  [
    'Between',
    'Over',
    'Under'
  ];

class Filter extends Component{
  constructor(props){
    super(props);

    this.state = {
      selectedfilterOptionKey: 1,
      higherThan: 0,
      lowerThan: 100,
      singleNumericFilterValue: 0,
      stringFilter: ''
    }
  }

  render(){
    const {props} = this;

    if (props.type === "number"){
      return this.renderNumericFilter();
    }
    else if (props.type === "text"){
      return this.renderTextFilter();
    }

    return null;
  }

  renderNumericFilter = () => {
    const {state} = this;

    let form;

    if (state.selectedfilterOptionKey === 0){
      form = <Form>
              <FormControl className="filterSmall" type="number" value={state.higherThan} onChange={this.setValue.bind(this, "higherThan")}/>
              <FormControl className="filterSmall" type="number" value={state.lowerThan} onChange={this.setValue.bind(this, "lowerThan")}/>
            </Form>;
    }
    else {
      form = <Form>
              <FormControl className="filterLarge" type="number" value={state.singleNumericFilterValue}
                onChange={this.setValue.bind(this, "singleNumericFilterValue")}/>
            </Form>;
    }

    return(<FormGroup className="filter">
            <div className="selectOption">
              <Dropdown className="selectOption" defaultKey={1} stateKey={"selectedfilterOptionKey"} id={999}
                data={selections} title="Bet" onUpdate={this.updateSelection}/>
              </div>
            {form}
          </FormGroup>);
  }

  renderTextFilter = () => {
    const {props, state} = this;

    return(
      <Form>
          <ControlLabel className="filterSmall">{props.label}</ControlLabel>
          <FormControl className="filterLarge" type="text" value={state.stringFilter}
           onChange={this.setValue.bind(this, "stringFilter")}/>
        </Form>);
  }

  /*
    Updates state key with new value.
  */
  updateSelection = (value, key) => {
    this.setState({
      [key]: value
    })
  }

  setValue = (param, e) => {
    this.setState({
      [param]: e.target.value
    });
  }
}

/*Filter.propTypes = {
  arrayToFilter: PropTypes.array.isRequired,
  filteredKey: PropTypes.string.isRequired
};*/

Filter.propTypes = {
  label: PropTypes.string,
  type: PropTypes.string.isRequired //'text', 'number'
};

export default Filter;
