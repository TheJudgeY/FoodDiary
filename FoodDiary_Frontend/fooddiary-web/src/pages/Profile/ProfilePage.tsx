import React, { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { 
  User as UserIcon, 
  Save, 
  Target, 
  Scale, 
  Activity, 
  TrendingUp,
  Info,
  CheckCircle,
  AlertCircle
} from 'lucide-react';
import { useAuthStore } from '../../stores/authStore';
import { authService } from '../../services/authService';
import { User, Gender, ActivityLevel, FitnessGoal } from '../../types';
import { formatNumber, getGenderLabel, getGenderFromNumber, getActivityLevelLabel, getActivityLevelFromNumber, getFitnessGoalLabel, getFitnessGoalFromNumber } from '../../utils';
import Card from '../../components/UI/Card';
import Button from '../../components/UI/Button';
import Input from '../../components/UI/Input';
import LoadingSpinner from '../../components/UI/LoadingSpinner';


const profileSchema = z.object({
  name: z.string().min(1, 'Name is required').max(100, 'Name must be less than 100 characters'),
  heightCm: z.number().min(100, 'Height must be at least 100cm').max(250, 'Height must be less than 250cm'),
  weightKg: z.number().min(30, 'Weight must be at least 30kg').max(300, 'Weight must be less than 300kg'),
  age: z.number().min(13, 'Age must be at least 13').max(120, 'Age must be less than 120'),
  gender: z.nativeEnum(Gender),
  activityLevel: z.nativeEnum(ActivityLevel),
  fitnessGoal: z.nativeEnum(FitnessGoal),
  targetWeightKg: z.number().min(30, 'Target weight must be at least 30kg').max(300, 'Target weight must be less than 300kg'),
  dailyCalorieGoal: z.number().min(800, 'Daily calorie goal must be at least 800').max(5000, 'Daily calorie goal must be less than 5000'),
  dailyProteinGoal: z.number().min(20, 'Daily protein goal must be at least 20g').max(500, 'Daily protein goal must be less than 500g'),
  dailyFatGoal: z.number().min(20, 'Daily fat goal must be at least 20g').max(200, 'Daily fat goal must be less than 200g'),
  dailyCarbohydrateGoal: z.number().min(50, 'Daily carbohydrate goal must be at least 50g').max(800, 'Daily carbohydrate goal must be less than 800g'),
});

type ProfileFormData = z.infer<typeof profileSchema>;

const ProfilePage: React.FC = () => {
  const { user, updateUser } = useAuthStore();
  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [saveStatus, setSaveStatus] = useState<'idle' | 'success' | 'error'>('idle');
  const [calculatedValues, setCalculatedValues] = useState({
    bmi: 0,
    bmiCategory: '',
    bmr: 0,
    tdee: 0,
    recommendedCalories: 0,
  });

  const {
    register,
    handleSubmit,
    watch,
    setValue,
    reset,
    formState: { errors, isDirty },
  } = useForm<ProfileFormData>({
    resolver: zodResolver(profileSchema),
  });

  
  const heightCm = watch('heightCm');
  const weightKg = watch('weightKg');
  const age = watch('age');
  const gender = watch('gender');
  const activityLevel = watch('activityLevel');
  const fitnessGoal = watch('fitnessGoal');

  
  useEffect(() => {
    if (heightCm && weightKg && age && gender !== undefined && activityLevel !== undefined && fitnessGoal !== undefined) {
      
      const heightM = heightCm / 100;
      const bmi = weightKg / (heightM * heightM);
      
      
      let bmiCategory = '';
      if (bmi < 18.5) bmiCategory = 'Underweight';
      else if (bmi < 25) bmiCategory = 'Normal weight';
      else if (bmi < 30) bmiCategory = 'Overweight';
      else bmiCategory = 'Obese';

      
      let bmr = 0;
      if (gender === Gender.Male) {
        bmr = 10 * weightKg + 6.25 * heightCm - 5 * age + 5;
      } else {
        bmr = 10 * weightKg + 6.25 * heightCm - 5 * age - 161;
      }

      
      const activityMultipliers = {
        [ActivityLevel.Sedentary]: 1.2,
        [ActivityLevel.LightlyActive]: 1.375,
        [ActivityLevel.ModeratelyActive]: 1.55,
        [ActivityLevel.VeryActive]: 1.725,
        [ActivityLevel.ExtremelyActive]: 1.9,
      };
      
      const tdee = bmr * activityMultipliers[activityLevel as ActivityLevel];

      
      let recommendedCalories = tdee;
      if (fitnessGoal === FitnessGoal.LoseWeight) {
        recommendedCalories = tdee * 0.85; 
      } else if (fitnessGoal === FitnessGoal.GainWeight) {
        recommendedCalories = tdee * 1.15; 
      }

      setCalculatedValues({
        bmi: Math.round(bmi * 10) / 10,
        bmiCategory,
        bmr: Math.round(bmr),
        tdee: Math.round(tdee),
        recommendedCalories: Math.round(recommendedCalories),
      });
    } else {
      
      setCalculatedValues({
        bmi: 0,
        bmiCategory: '',
        bmr: 0,
        tdee: 0,
        recommendedCalories: 0,
      });
    }
  }, [heightCm, weightKg, age, gender, activityLevel, fitnessGoal]);

  
  useEffect(() => {
    const loadUserData = async () => {
      if (user) {
        setIsLoading(true);
        try {
          const response = await authService.getCurrentUser();
          console.log('API response:', response);
          
          
          const userData = (response as { user?: User }).user || response as User;
          console.log('Extracted user data:', userData);
          
          if (userData) {
            console.log('Loading user data:', userData);
            
            
            const formData = {
              name: userData.name || '',
              heightCm: userData.heightCm || undefined,
              weightKg: userData.weightKg || undefined,
              age: userData.age || undefined,
              gender: userData.gender !== null && userData.gender !== undefined ? getGenderFromNumber(userData.gender) : undefined,
              activityLevel: userData.activityLevel !== null && userData.activityLevel !== undefined ? getActivityLevelFromNumber(userData.activityLevel) : undefined,
              fitnessGoal: userData.fitnessGoal !== null && userData.fitnessGoal !== undefined ? getFitnessGoalFromNumber(userData.fitnessGoal) : undefined,
              targetWeightKg: userData.targetWeightKg || undefined,
              dailyCalorieGoal: userData.dailyCalorieGoal || undefined,
              dailyProteinGoal: userData.dailyProteinGoal || undefined,
              dailyFatGoal: userData.dailyFatGoal || undefined,
              dailyCarbohydrateGoal: userData.dailyCarbohydrateGoal || undefined,
            };
            
            console.log('Form data to load:', formData);
            
            
            reset(formData);
          }
        } catch (error) {
          console.error('Failed to load user data:', error);
          setSaveStatus('error');
        } finally {
          setIsLoading(false);
        }
      }
    };

    loadUserData();
  }, [user, reset]);

  const onSubmit = async (data: ProfileFormData) => {
    setIsSaving(true);
    setSaveStatus('idle');
    
    try {
      
      await updateUser(data);
      setSaveStatus('success');
      
      
      setTimeout(() => setSaveStatus('idle'), 3000);
    } catch (error) {
      console.error('Failed to update profile:', error);
      setSaveStatus('error');
    } finally {
      setIsSaving(false);
    }
  };

  const handleAutoFillRecommended = () => {
    setValue('dailyCalorieGoal', calculatedValues.recommendedCalories);
    setValue('dailyProteinGoal', Math.round((calculatedValues.recommendedCalories * 0.25) / 4));
    setValue('dailyFatGoal', Math.round((calculatedValues.recommendedCalories * 0.30) / 9));
    setValue('dailyCarbohydrateGoal', Math.round((calculatedValues.recommendedCalories * 0.45) / 4));
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <LoadingSpinner size="lg" />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-surface-900">Profile</h1>
          <p className="text-surface-600">Manage your personal information and fitness goals.</p>
        </div>
        <div className="flex items-center gap-2">
          {saveStatus === 'success' && (
            <div className="flex items-center gap-2 text-green-600">
              <CheckCircle className="h-4 w-4" />
              <span className="text-sm">Profile updated successfully!</span>
            </div>
          )}
          {saveStatus === 'error' && (
            <div className="flex items-center gap-2 text-red-600">
              <AlertCircle className="h-4 w-4" />
              <span className="text-sm">Failed to update profile</span>
            </div>
          )}
        </div>
      </div>



      <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
        {/* Personal Information */}
        <Card>
          <div className="p-6">
            <div className="flex items-center gap-3 mb-6">
              <div className="w-10 h-10 bg-primary-100 rounded-lg flex items-center justify-center">
                <UserIcon className="h-5 w-5 text-primary-600" />
              </div>
              <div>
                <h2 className="text-lg font-semibold text-surface-900">Personal Information</h2>
                <p className="text-sm text-surface-600">Basic information about yourself</p>
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label className="block text-sm font-medium text-surface-700 mb-2">
                  Full Name *
                </label>
                <Input
                  {...register('name')}
                  placeholder="Enter your full name"
                  error={errors.name?.message}
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-surface-700 mb-2">
                  Age *
                </label>
                <Input
                  {...register('age', { valueAsNumber: true })}
                  type="number"
                  placeholder="Enter your age"
                  error={errors.age?.message}
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-surface-700 mb-2">
                  Gender *
                </label>
                <select
                  {...register('gender', { valueAsNumber: true })}
                  className="w-full px-3 py-2 border border-surface-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                >
                  <option value="">Select gender</option>
                  <option value={Gender.Male}>{getGenderLabel(Gender.Male)}</option>
                  <option value={Gender.Female}>{getGenderLabel(Gender.Female)}</option>
                  <option value={Gender.Other}>{getGenderLabel(Gender.Other)}</option>
                </select>
              </div>
            </div>
          </div>
        </Card>

        {/* Body Metrics */}
        <Card>
          <div className="p-6">
            <div className="flex items-center gap-3 mb-6">
              <div className="w-10 h-10 bg-green-100 rounded-lg flex items-center justify-center">
                <Scale className="h-5 w-5 text-green-600" />
              </div>
              <div>
                <h2 className="text-lg font-semibold text-surface-900">Body Metrics</h2>
                <p className="text-sm text-surface-600">Your current body measurements</p>
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label className="block text-sm font-medium text-surface-700 mb-2">
                  Height (cm) *
                </label>
                <Input
                  {...register('heightCm', { valueAsNumber: true })}
                  type="number"
                  placeholder="Enter your height in cm"
                  error={errors.heightCm?.message}
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-surface-700 mb-2">
                  Weight (kg) *
                </label>
                <Input
                  {...register('weightKg', { valueAsNumber: true })}
                  type="number"
                  step="0.1"
                  placeholder="Enter your weight in kg"
                  error={errors.weightKg?.message}
                />
              </div>
            </div>

            {/* Calculated Values */}
            {calculatedValues.bmi > 0 && (
              <div className="mt-6 p-4 bg-surface-50 rounded-lg">
                <h3 className="text-sm font-medium text-surface-700 mb-3">Calculated Values</h3>
                <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                  <div>
                    <p className="text-xs text-surface-500">BMI</p>
                    <p className="text-lg font-semibold text-surface-900">{calculatedValues.bmi}</p>
                    <p className="text-xs text-surface-600">{calculatedValues.bmiCategory}</p>
                  </div>
                  <div>
                    <p className="text-xs text-surface-500">BMR</p>
                    <p className="text-lg font-semibold text-surface-900">{formatNumber(calculatedValues.bmr)} kcal</p>
                    <p className="text-xs text-surface-600">Basal Metabolic Rate</p>
                  </div>
                  <div>
                    <p className="text-xs text-surface-500">TDEE</p>
                    <p className="text-lg font-semibold text-surface-900">{formatNumber(calculatedValues.tdee)} kcal</p>
                    <p className="text-xs text-surface-600">Total Daily Energy Expenditure</p>
                  </div>
                  <div>
                    <p className="text-xs text-surface-500">Recommended</p>
                    <p className="text-lg font-semibold text-surface-900">{formatNumber(calculatedValues.recommendedCalories)} kcal</p>
                    <p className="text-xs text-surface-600">Based on your goal</p>
                  </div>
                </div>
              </div>
            )}
          </div>
        </Card>

        {/* Activity & Goals */}
        <Card>
          <div className="p-6">
            <div className="flex items-center gap-3 mb-6">
              <div className="w-10 h-10 bg-blue-100 rounded-lg flex items-center justify-center">
                <Activity className="h-5 w-5 text-blue-600" />
              </div>
              <div>
                <h2 className="text-lg font-semibold text-surface-900">Activity & Goals</h2>
                <p className="text-sm text-surface-600">Your activity level and fitness objectives</p>
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label className="block text-sm font-medium text-surface-700 mb-2">
                  Activity Level *
                </label>
                <select
                  {...register('activityLevel', { valueAsNumber: true })}
                  className="w-full px-3 py-2 border border-surface-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                >
                  <option value="">Select activity level</option>
                  <option value={ActivityLevel.Sedentary}>{getActivityLevelLabel(ActivityLevel.Sedentary)} (little or no exercise)</option>
                  <option value={ActivityLevel.LightlyActive}>{getActivityLevelLabel(ActivityLevel.LightlyActive)} (light exercise/sports 1-3 days/week)</option>
                  <option value={ActivityLevel.ModeratelyActive}>{getActivityLevelLabel(ActivityLevel.ModeratelyActive)} (moderate exercise/sports 3-5 days/week)</option>
                  <option value={ActivityLevel.VeryActive}>{getActivityLevelLabel(ActivityLevel.VeryActive)} (hard exercise/sports 6-7 days a week)</option>
                  <option value={ActivityLevel.ExtremelyActive}>{getActivityLevelLabel(ActivityLevel.ExtremelyActive)} (very hard exercise/sports & physical job)</option>
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-surface-700 mb-2">
                  Fitness Goal *
                </label>
                <select
                  {...register('fitnessGoal', { valueAsNumber: true })}
                  className="w-full px-3 py-2 border border-surface-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                >
                  <option value="">Select fitness goal</option>
                  <option value={FitnessGoal.LoseWeight}>{getFitnessGoalLabel(FitnessGoal.LoseWeight)}</option>
                  <option value={FitnessGoal.MaintainWeight}>{getFitnessGoalLabel(FitnessGoal.MaintainWeight)}</option>
                  <option value={FitnessGoal.GainWeight}>{getFitnessGoalLabel(FitnessGoal.GainWeight)}</option>
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-surface-700 mb-2">
                  Target Weight (kg) *
                </label>
                <Input
                  {...register('targetWeightKg', { valueAsNumber: true })}
                  type="number"
                  step="0.1"
                  placeholder="Enter your target weight"
                  error={errors.targetWeightKg?.message}
                />
              </div>
            </div>
          </div>
        </Card>

        {/* Nutritional Goals */}
        <Card>
          <div className="p-6">
            <div className="flex items-center justify-between mb-6">
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 bg-orange-100 rounded-lg flex items-center justify-center">
                  <Target className="h-5 w-5 text-orange-600" />
                </div>
                <div>
                  <h2 className="text-lg font-semibold text-surface-900">Nutritional Goals</h2>
                  <p className="text-sm text-surface-600">Your daily macronutrient targets</p>
                </div>
              </div>
              <Button
                type="button"
                variant="outline"
                size="sm"
                onClick={handleAutoFillRecommended}
                className="flex items-center gap-2"
              >
                <TrendingUp className="h-4 w-4" />
                Auto-fill Recommended
              </Button>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label className="block text-sm font-medium text-surface-700 mb-2">
                  Daily Calorie Goal (kcal) *
                </label>
                <Input
                  {...register('dailyCalorieGoal', { valueAsNumber: true })}
                  type="number"
                  placeholder="Enter daily calorie goal"
                  error={errors.dailyCalorieGoal?.message}
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-surface-700 mb-2">
                  Daily Protein Goal (g) *
                </label>
                <Input
                  {...register('dailyProteinGoal', { valueAsNumber: true })}
                  type="number"
                  step="0.1"
                  placeholder="Enter daily protein goal"
                  error={errors.dailyProteinGoal?.message}
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-surface-700 mb-2">
                  Daily Fat Goal (g) *
                </label>
                <Input
                  {...register('dailyFatGoal', { valueAsNumber: true })}
                  type="number"
                  step="0.1"
                  placeholder="Enter daily fat goal"
                  error={errors.dailyFatGoal?.message}
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-surface-700 mb-2">
                  Daily Carbohydrate Goal (g) *
                </label>
                <Input
                  {...register('dailyCarbohydrateGoal', { valueAsNumber: true })}
                  type="number"
                  step="0.1"
                  placeholder="Enter daily carbohydrate goal"
                  error={errors.dailyCarbohydrateGoal?.message}
                />
              </div>
            </div>

            {/* Info Box */}
            <div className="mt-6 p-4 bg-blue-50 border border-blue-200 rounded-lg">
              <div className="flex items-start gap-3">
                <Info className="h-5 w-5 text-blue-600 mt-0.5 flex-shrink-0" />
                <div>
                  <p className="text-sm font-medium text-blue-800 mb-1">Nutritional Guidelines</p>
                  <ul className="text-xs text-blue-700 space-y-1">
                    <li>• Protein: 25% of daily calories (4 calories per gram)</li>
                    <li>• Fat: 30% of daily calories (9 calories per gram)</li>
                    <li>• Carbohydrates: 45% of daily calories (4 calories per gram)</li>
                    <li>• Use the "Auto-fill Recommended" button to set goals based on your profile</li>
                  </ul>
                </div>
              </div>
            </div>
          </div>
        </Card>

        {/* Save Button */}
        <div className="flex justify-end">
          <Button
            type="submit"
            disabled={isSaving || !isDirty}
            className="flex items-center gap-2"
          >
            {isSaving ? (
              <LoadingSpinner size="sm" />
            ) : (
              <Save className="h-4 w-4" />
            )}
            {isSaving ? 'Saving...' : 'Save Profile'}
          </Button>
        </div>
      </form>
    </div>
  );
};

export default ProfilePage;
