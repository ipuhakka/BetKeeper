import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { BarChart, Bar, CartesianGrid, XAxis, YAxis, Legend, ResponsiveContainer } from 'recharts';
import Dropdown from '../../components/Dropdown/Dropdown.jsx';

class BarGraph extends Component {
  constructor(props){
    super(props);

    this.state = {
      selectedVariable: 0
    };
  }

  render(){
    return(
      <div>
        <Dropdown
          bsStyle="primary"
          title={"Overview"}
          id={1}
          data={this.props.optionLabels}
          onUpdate={this.setSelectedDropdownItem.bind(this)}
          stateKey={"selectedVariable"}>
        </Dropdown>
  			<div className="chart">
  				<ResponsiveContainer>
  					<BarChart barSize={20} barGap={2} data={this.props.data.slice()}>
  						<CartesianGrid strokeDasharray="3 3" />
  						<XAxis dataKey="folder" />
  						<YAxis />
  						<Legend />
  						<Bar name={this.props.graphOptions[this.state.selectedVariable].labelName} dataKey={this.props.graphOptions[this.state.selectedVariable].variableName} fill="#8884d8" />
  					</BarChart>
  				</ResponsiveContainer>
  			</div>
      </div>
		);
  }

  setSelectedDropdownItem = (key, stateKey) => {
    this.setState({
      [stateKey]: key
    });
  }
};

BarGraph.propTypes = {
  graphOptions: PropTypes.array.isRequired,
  optionLabels: PropTypes.array.isRequired,
  data: PropTypes.array.isRequired
};

export default BarGraph;
