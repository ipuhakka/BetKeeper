import React, { Component } from 'react';
import PropTypes from 'prop-types';
import DefaultTooltipContent from 'recharts/lib/component/DefaultTooltipContent';
import { Legend, Label, Tooltip, Scatter, ScatterChart, CartesianGrid, XAxis, YAxis, ResponsiveContainer } from 'recharts';
import Dropdown from '../../components/Dropdown/Dropdown.jsx';
import './ScatterPlot.css';

const plotColors = [
  'black', 'blue', 'red', 'green', 'yellow', 'orange', 'purple',
  'cyan', 'darkRed'
];

class ScatterPlot extends Component{
  constructor(props){
    super(props);

    this.state = {
      xVariable: 0,
      yVariable: 1,
    }
  }

  render(){
    let i = 0;
    let scatters = this.props.data.map(folder => {
      let arr = [folder];
      let scatter = (<Scatter name={folder.folder} key={i} data={arr} fill={i >= plotColors.length ? '#' + Math.floor(Math.random() * 16777215).toString(16) : plotColors[i]} />);
      i = i + 1;
      return scatter;
    });

    return (
      <div>
        <div className='selection-div'>
          <Dropdown
            className='margin-right'
            variant="primary"
            title={"Y"}
            selectedItemAsTitle
            id={3}
            data={this.props.optionLabels}
            onUpdate={this.setSelectedDropdownItem.bind(this)}
            defaultKey={1}
            stateKey={"yVariable"}>
          </Dropdown>
          <Dropdown
            variant="primary"
            title={"X"}
            id={4}       
            selectedItemAsTitle
            data={this.props.optionLabels}
            onUpdate={this.setSelectedDropdownItem.bind(this)}
            stateKey={"xVariable"}>
          </Dropdown>
        </div>

        <div className="scatterplot-div chart">
          <ResponsiveContainer>
            <ScatterChart>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis 
                type="number" 
                name={this.props.graphOptions[this.state.xVariable].labelName} 
                dataKey={this.props.graphOptions[this.state.xVariable].variableName}>
                <Label 
                  offset={-5} 
                  position="insideBottom" 
                  value={this.props.graphOptions[this.state.xVariable].labelName} />
              </XAxis>
              <YAxis 
                type="number" 
                name={this.props.graphOptions[this.state.yVariable].labelName} 
                dataKey={this.props.graphOptions[this.state.yVariable].variableName}>
                <Label 
                  angle={-90} 
                  position="insideBottomLeft" 
                  value={this.props.graphOptions[this.state.yVariable].labelName} />
              </YAxis>
              <Tooltip content={this.customTooltip}/>
              {scatters}
              <Legend offset={-5}/>
            </ScatterChart>
          </ResponsiveContainer>
        </div>
      </div>
    );
  }

  customTooltip = (props) => {
    if (props.payload.length > 0){
      const newPayload = [
        {
          name: "Folder",
          value: props.payload[0].payload.folder
        },
        ...props.payload,
      ];
      return <DefaultTooltipContent {...props} payload={newPayload} />;
    }
    return <DefaultTooltipContent {...props} />;
  }

  setSelectedDropdownItem = (key, stateKey) => {
    this.setState({
      [stateKey]: key
    });
  }
};

ScatterPlot.propTypes = {
  graphOptions: PropTypes.array.isRequired,
  optionLabels: PropTypes.array.isRequired,
  data: PropTypes.array.isRequired
}

export default ScatterPlot;
