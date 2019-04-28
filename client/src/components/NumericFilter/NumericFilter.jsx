import React, { Component } from 'react';
import PropTypes from 'prop-types';
import ControlLabel from 'react-bootstrap/lib/ControlLabel';
import Form from 'react-bootstrap/lib/Form';
import FormControl from 'react-bootstrap/lib/FormControl';
import FormGroup from 'react-bootstrap/lib/FormGroup';
import Dropdown from '../Dropdown/Dropdown.jsx';
import './NumericFilter.css';

const selections =
  [
    'Between',
    'Over',
    'Under'
  ];

class NumericFilter extends Component{
  constructor(props){
    super(props);

    this.state = {
      selectedfilterOptionKey: 1,
      higherThan: 0,
      lowerThan: 100,
      singleFilterValue: 0
    }
  }

  render(){
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
              <FormControl className="filterLarge" type="number" value={state.singleFilterValue}
                onChange={this.setValue.bind(this, "singleFilterValue")}/>
            </Form>;
    }

    return(<FormGroup className="numericFilter">
            <div className="selectOption">
              <Dropdown className="selectOption" defaultKey={1} stateKey={"selectedfilterOptionKey"} id={999}
                data={selections} title="Bet" onUpdate={this.updateSelection}/>
              </div>
            {form}
          </FormGroup>);
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

/*NumericFilter.propTypes = {
  arrayToFilter: PropTypes.array.isRequired,
  filteredKey: PropTypes.string.isRequired
};*/

export default NumericFilter;
